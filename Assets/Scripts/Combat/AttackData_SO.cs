using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    //近战距离
    public float attackRange;
    //远程距离
    public float skillRange;
    //冷却时间
    public float coolDown;
    //浮动伤害
    public int minDamage;
    public int maxDamage;
    //暴击加成（基础 * 加成）
    public float criticalMultiplier;
    //暴击率（%）
    public float criticalChance;
}
