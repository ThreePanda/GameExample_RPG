using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        //将移动的事件注册到单例的event事件OnMouseCliked中，随事件唤醒调用
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += Event_AttackEnemy;
    }

    private GameObject _attackEnemy;
    private float _lastAttackTime;
    private void Event_AttackEnemy(GameObject target)
    {
        if (target != null)
        {
            _attackEnemy = target;
        }
        StartCoroutine(MoveToAttackTarget());
        //启用迭代器返回值的函数内的循环(yield return)，每帧执行。（等于在update内执行）
        //参考Coroutines的手册
    }

    private void Update()
    {
        SwitchAnimation();
        //攻击CD冷却衰减
        _lastAttackTime -= Time.deltaTime;
    }

    private void MoveToTarget(Vector3 target)
    {
        //恢复攻击设置的停止
        _agent.isStopped = false;
        //打断攻击移动导航
        StopAllCoroutines();
        _agent.destination = target;
    }

    IEnumerator MoveToAttackTarget()
    {
        //transform.LookAt(_attackEnemy.transform);
        while (Vector3.Distance(_attackEnemy.transform.position,transform.position) > 1)
        {//当人物与敌人的距离大于1时，控制人物移动到敌人处(1参考了Play的提前停止参数)TODO:根据武器修改参数
            _agent.isStopped = false;
            _agent.destination = _attackEnemy.transform.position;
            yield return null;
            //IEnumerator(迭代器)下的 yield return 关键字可以保存循环的状态，每请求一次迭代器的值返回一次循环的 return 结果，直到循环结束
            //return只能在循环外部一次性地返回，yeild则可以在循环内部逐个元素返回
            //执行流程：main -> while -> return -> main -> while -> return -> main...
            //提高输出的效率，本质与先保存所有值然后再输出的速度无异
        }
        _agent.isStopped = true;

        if (_lastAttackTime < 0)
        {
            _animator.SetTrigger("Attack");
            //重置攻击CD
            _lastAttackTime = 0.5f;
        }
    }

    private void SwitchAnimation()
    {
        _animator.SetFloat("Speed", _agent.velocity.sqrMagnitude);
    }
}
