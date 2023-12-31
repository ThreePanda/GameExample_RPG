using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[CreateAssetMenu(fileName = "New Data",menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")] 
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;
    [Header("Level")] 
    public int currentLevel;
    public int maxLevel;
    //等级提升所需经验
    public int baseExp;
    public int currentExp;
    //属性提升百分比
    public float levelBuff;
    [Header("Monster Exp")] 
    //怪物死亡所给予的经验值
    public int killPoint;

    //下一阶段升级乘数
    private float LevelMultiplier => 1 + (currentLevel - 1) * levelBuff;

    public void UpdateExp(int point)
    {
        currentExp += point;
        //打死一个可以连升
        while (currentExp >= baseExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        baseExp += (int)(baseExp * LevelMultiplier);
        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;
        Debug.Log("Level has changed! Now: " + currentLevel + "  Max health: " + maxHealth);
    }
}
