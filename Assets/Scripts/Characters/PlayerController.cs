using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        //将移动的事件注册到单例的event事件OnMouseCliked中，随事件唤醒调用
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
    }
    public void MoveToTarget(Vector3 target)
    {
        _agent.destination = target;
    }
}
