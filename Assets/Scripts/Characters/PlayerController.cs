using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    private CharacterStats _characterStats;
    private bool _isDead;
    private GameObject _attackEnemy;
    private float _lastAttackTime;
    private float _stopDistance;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _characterStats = GetComponent<CharacterStats>();
    }

    private void OnEnable()
    {
        //将移动的事件注册到单例的event事件OnMouseCliked中，随事件唤醒调用
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += Event_AttackEnemy;
    }

    private void Start()
    {
        GameManager.Instance.RegisterPlayer(_characterStats);
    }

    private void OnDisable()
    {
        if (!MouseManager.isInitialize) return;
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.OnEnemyClicked -= Event_AttackEnemy;
    }


    private void Update()
    {
        //死亡判断
        _isDead = _characterStats.CurrentHealth == 0;
        SwitchAnimation();
        //攻击CD冷却衰减
        _lastAttackTime -= Time.deltaTime;
    } 
    private void SwitchAnimation()
    {
        _animator.SetFloat("Speed", _agent.velocity.sqrMagnitude);
        _animator.SetBool("Death",_isDead);
    }
    private void Event_AttackEnemy(GameObject target)
    {
        if (_isDead) return;
        if (target != null)
        {
            _attackEnemy = target;
        }
        //TODO:鼠标每点击敌人一次，Coroutine就会执行一次，浪费资源，违背协程的刷新机制
        StartCoroutine(MoveToAttackTarget());
        //启用迭代器返回值的函数内的循环(yield return)，每帧执行。（等于在update内执行）
        //参考Coroutines的手册
    }
    private void MoveToTarget(Vector3 target)
    {
        if (_isDead) return;
        //恢复攻击设置的停止
        _agent.isStopped = false;
        //打断攻击移动导航
        StopAllCoroutines();
        _agent.destination = target;
    }

    private IEnumerator MoveToAttackTarget()
    {
        //当人物与敌人的距离大于攻击距离+敌人半径时，控制人物移动到敌人处
        //加敌人半径是因为当攻击距离 < 目标碰撞体半径时会导致死循环
        //因为Rock没有NavMeshAgent导致报错，距离判断radius已修改
        while (Vector3.Distance(_attackEnemy.transform.position,transform.position) 
               > _characterStats.attackData.attackRange 
               + (_attackEnemy.CompareTag("Enemy") ? _attackEnemy.GetComponent<NavMeshAgent>().radius : 1))
        {
            _agent.isStopped = false;
            _agent.destination = _attackEnemy.transform.position;
            yield return null;
            //IEnumerator(迭代器)下的 yield return 关键字可以保存循环的状态，每请求一次迭代器的值返回一次循环的 return 结果，直到循环结束
            //return只能在循环外部一次性地返回，yeild则可以在循环内部逐个元素返回
            //执行流程：main -> while -> return -> main -> while -> return -> main...
            //提高输出的效率，本质与先保存所有值然后再输出的速度无异
        }
        _agent.isStopped = true;
        Attack();
    }

    void Attack()
    {
        if (_lastAttackTime < 0)
        {
            transform.LookAt(_attackEnemy.transform);
            //重置攻击CD
            _lastAttackTime = _characterStats.attackData.coolDown;
            //暴击判断
            //Random.value的值视为百分数，小于给定的暴击率则表明命中暴击概率
            _characterStats.isCritical = 
                Random.value < _characterStats.attackData.criticalChance;
            _animator.SetBool("Critical",_characterStats.isCritical);
            _animator.SetTrigger("Attack");
        }
    }
   
    
    //Animation Event
    void Hit()
    {
        if (_attackEnemy.CompareTag("Attackable"))
        {
            if (_attackEnemy.GetComponent<Rock>())
            {
                _attackEnemy.GetComponent<Rock>().rockStats = Rock.RockStats.HitEnemy;
                //TODO:临时测试版：击飞力大小
                _attackEnemy.GetComponent<Rigidbody>().velocity = Vector3.one;
                _attackEnemy.GetComponent<Rigidbody>().AddForce(transform.forward * 20,ForceMode.Impulse);
            }
        }
        else
        {
            //Enemy在点击的Event事件中已经赋值
            var targetStats = _attackEnemy.GetComponent<CharacterStats>();
            targetStats.TakeDamage(_characterStats, targetStats);
        }
    }
}
