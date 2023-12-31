using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.Serialization;

public class Rock : MonoBehaviour
{
    public enum RockStats
    {
        HitPlayer,
        HitEnemy,
        HitNoting
    }
    private Rigidbody _rigidbody;
    private Vector3 _direction;
    public RockStats rockStats;
    [Header("Basic Setting")] public float force;
    [HideInInspector]public GameObject target;
    public int damage;
    public GameObject breakEffect;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        //给一个初始速度，方便判断何时停止
        _rigidbody.velocity = Vector3.one;
        rockStats = RockStats.HitPlayer;
        FlyToTarget();
    }
    //rigidbody刚体采用此方法
    private void FixedUpdate()
    {
        if (_rigidbody.velocity.sqrMagnitude < 1f)
        {
            rockStats = RockStats.HitNoting;
        }
    }

    private void FlyToTarget()
    {
        //给一个垂直向上的量，帮助玩家躲避，提高游戏体验
        _direction = (target.transform.position - transform.position + Vector3.up).normalized;
        _rigidbody.AddForce(_direction * force,ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (rockStats)
        {
            case RockStats.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = _direction * force;
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy"); 
                    //避免被击飞后重新回到原来的位置
                    other.gameObject.GetComponent<NavMeshAgent>().ResetPath();
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage,other.gameObject.GetComponent<CharacterStats>());
                    rockStats = RockStats.HitNoting;
                }
                break;
            case RockStats.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage,otherStats);
                    Instantiate(breakEffect, transform.position, quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
