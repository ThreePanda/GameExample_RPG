using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    //技能击飞力度
    [Header("Skill")] public float kickForce = 10;
    public GameObject rockPrefab;
    public Transform handPos;
    //Animation Event
    public void KickOff()
    {
        if (attackTarget == null || !transform.IsFacingTarget(attackTarget.transform)) return;
        
        var targetStats = attackTarget.GetComponent<CharacterStats>();
        Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
        //direction.Normalize();
        targetStats.GetComponent<NavMeshAgent>().isStopped = true;
        targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
        targetStats.TakeDamage(_characterStats, targetStats);
        targetStats.GetComponent<Animator>().SetTrigger("Dizzy");
        targetStats.GetComponent<NavMeshAgent>().ResetPath();
    }
    //Animation Event
    public void ThrowRock()
    {
        if (attackTarget == null) return;
        var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
        rock.GetComponent<Rock>().target = attackTarget;
    }
}
