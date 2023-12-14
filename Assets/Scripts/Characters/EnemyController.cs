using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyStates
{
    GUARD,
    PATROL,
    CHASE,
    DEAD
};
[RequireComponent(typeof(NavMeshAgent))]
//检查对应的Component是否存在，若不存在自动添加
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private EnemyStates _enemyStates;
    private GameObject _attackTarget;
    private Animator _animator;
    private float _chaseAgentSpeed;
    private Vector3 _guradPos;
    private CharacterStats _characterStats;
    private Quaternion _guardRotation;
    private Collider _collider;
    //CD冷却时间
    private float _lastAttackTime;

    
    [Header("Basic Settings")] 
    //探测范围
    public float sightRadius;
    //当前状态是否为站桩
    public bool isGuard;
    //观察持续时间
    public float waitTime;
    private float _remainWaitTime;
    [Header("Patrol State")] 
    //巡逻范围
    public float patrolRange;
    
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _characterStats = GetComponent<CharacterStats>();
        _collider = GetComponent<Collider>();
        _chaseAgentSpeed = _agent.speed;
        _guradPos = transform.position;
        _remainWaitTime = waitTime;
        _guardRotation = transform.rotation;

    }

    private void Start()
    {
        if (isGuard)
        {
            _enemyStates = EnemyStates.GUARD;
        }
        else
        {
            _enemyStates = EnemyStates.PATROL;
            //初始化waypoint
            GetNewWayPoint();
        }
    }

    private void Update()
    {
        //死亡判断
        _isDead = _characterStats.CurrentHealth == 0;
        
        SwitchStates();
        SwitchAnimation();
        //攻击冷却
        _lastAttackTime -= Time.deltaTime;
    }
    //根据给定的等待时间等待
    bool Await()
    {
        if (_remainWaitTime >= 0)
        {
            _remainWaitTime -= Time.deltaTime;
            return true;
        }
        _remainWaitTime = waitTime;
        return false;
    }
    //bool类状态机选择
    private bool _isWalk;
    private bool _isChase;
    private bool _isFollow;
    private bool _isDead;
    void SwitchAnimation()
    {
        _animator.SetBool("Walk", _isWalk);
        _animator.SetBool("Chase", _isChase);
        _animator.SetBool("Follow", _isFollow);
        _animator.SetBool("Critical", _characterStats.isCritical);
        _animator.SetBool("Death", _isDead);
    }
    
    void SwitchStates()
    {
        if (_isDead)
        {
            _enemyStates = EnemyStates.DEAD;
        }
        else if (FoundPlayer())
        {
            _enemyStates = EnemyStates.CHASE;
        }
        switch (_enemyStates)
        {
            case EnemyStates.GUARD:
                _isChase = false;

                if (transform.position != _guradPos)
                {
                    _isWalk = true;
                    //_agent.isStopped = false;
                    _agent.destination = _guradPos;

                    if (Vector3.SqrMagnitude(_guradPos - transform.position)
                        <= _agent.stoppingDistance * _agent.stoppingDistance)
                    {
                        _isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation,_guardRotation,0.01f);
                    }
                }
                break;
            case EnemyStates.PATROL:
                _isWalk = true;
                _isChase = false;
                _agent.speed = _chaseAgentSpeed * 0.5f;
                //是否到达随机巡逻点
                if (Vector3.Distance(transform.position, _wayPoint) <= _agent.stoppingDistance)
                {
                    _isWalk = false;
                    if (!Await())
                    {
                        GetNewWayPoint();
                    }
                }
                else
                {
                    _isWalk = true;
                    _agent.destination = _wayPoint;
                }
                break;
            case EnemyStates.CHASE:
                _isWalk = false;
                _isChase = true;
                _agent.speed = _chaseAgentSpeed;
                if (!FoundPlayer())
                {
                    _isChase = false;
                    _isFollow = false;
                    _agent.destination = transform.position;
                    //等待一段时间后再返回
                    if (!Await())
                    {
                        //_agent.destination = _guradPos;
                        //此处的_wayPoint意在替换GetNewPoint的结果，避免其进入PATROL状态后地点被刷新
                        _wayPoint = _guradPos;
                        _enemyStates = isGuard ? EnemyStates.GUARD : EnemyStates.PATROL;
                    }
                }
                else
                {
                    //是否已经进入攻击范围
                    if (!TargetInSkillRange() && !TargetInAttackRange())
                    {
                        _isFollow = true;
                        _agent.destination = _attackTarget.transform.position;
                    }
                    else
                    { 
                        _isFollow = false;
                        if (_lastAttackTime < 0)
                        {
                            _lastAttackTime = _characterStats.attackData.coolDown;
                            //暴击判断
                                //Random.value的值视为百分数，小于给定的暴击率则表明命中暴击概率
                            _characterStats.isCritical = 
                                Random.value < _characterStats.attackData.criticalChance;
                            Attack();
                        }
                    }
                }
                break;
            case EnemyStates.DEAD:
                //避免在死亡后消失前玩家仍能对尸体攻击（事件传入值为射线碰撞体的object）
                _collider.enabled = false;
                //TODO:临时测试方法，后续修改
                _agent.enabled = false;
                Destroy(gameObject,2f);
                break;
        }
    }

    void Attack()
    {
        //using Hit function(Animation Event) to control the Health of Player and Monster
        transform.LookAt(_attackTarget.transform);
        if (TargetInAttackRange())
        {
            _animator.SetTrigger("Attack");
        }

        if (TargetInSkillRange())
        {
            _animator.SetTrigger("Skill");
        }
    }
    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (!target.CompareTag("Player")) continue;
            _attackTarget = target.gameObject;
            return true;
        }

        _attackTarget = null;
        return false;
    }

    private bool TargetInAttackRange()
    {
        if (_attackTarget != null)
            return Vector3.Distance(_attackTarget.transform.position, transform.position) 
                   <= _characterStats.attackData.attackRange;
        return false;
    }
    private bool TargetInSkillRange()
    {
        if (_attackTarget != null)
            return Vector3.Distance(_attackTarget.transform.position, transform.position) 
                   <= _characterStats.attackData.skillRange;
        return false;
    }
    //在sence中展示设定值的影响范围
    private void OnDrawGizmosSelected()
    {
        //索敌范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,sightRadius);
        //巡逻范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,patrolRange);
    }
    //生成一个随机的Vector3坐标，保证其能够在NavMesh中
    private Vector3 _wayPoint;
    void GetNewWayPoint()
    {
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(randomX + _guradPos.x, transform.position.y,
            randomZ + _guradPos.z);
        //一个最接近目标点且在NavMesh中的点的信息
        _wayPoint = NavMesh.SamplePosition(randomPoint, out var hit, patrolRange, 1) ? hit.position : transform.position;
    }
    //Animation Event
    void Hit()
    {
        //离开攻击范围即停止，避免空引用和拉怪被攻击
        if (!TargetInAttackRange() && !TargetInSkillRange()) return;
        
        var targetStats = _attackTarget.GetComponent<CharacterStats>();
        targetStats.TakeDamage(_characterStats, targetStats);
    }
}
