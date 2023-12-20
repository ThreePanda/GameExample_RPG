using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    //技能击飞力度
    [Header("Skill")] public float kickForce = 10;
    

    void kickOff()
    {
        if (attackTarget == null) return;
        
        transform.LookAt(attackTarget.transform);
        //获得攻击目标相对于我的正负值
        Vector3 direction = attackTarget.transform.position - transform.position;
        direction.Normalize();
        //击退
        attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
        attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
        attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        attackTarget.GetComponent<NavMeshAgent>().ResetPath();
    }
}
