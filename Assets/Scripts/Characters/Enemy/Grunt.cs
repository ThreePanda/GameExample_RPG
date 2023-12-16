using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    //技能击飞力度
    [Header("Skill")] public float kickForce = 10;
    
    private void FixedUpdate()
    {
        Debug.Log(_enemyStates);
    }

    void kickOff()
    {
        if (attackTarget != null)
        {
            //todo:基类isGuard切换状态时效，我在Chase中会一直向Target，导致持续与Target碰撞
            transform.LookAt(attackTarget.transform);
            //获得攻击目标相对于我的正负值
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();
            //击退
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
