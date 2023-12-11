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
    }

    private void Update()
    {
        SwitchAnimation();
    }

    public void MoveToTarget(Vector3 target)
    {
        _agent.destination = target;
    }

    public void SwitchAnimation()
    {
        _animator.SetFloat("Speed", _agent.velocity.sqrMagnitude);
    }
}
