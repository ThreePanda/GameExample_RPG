using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdataHealthOnAttack; 
    //设定一个模板，每一个角色从这个模板中复制一份，避免“全图秒杀”
    public CharacterData_SO templateData;
    
    public AttackData_SO attackData;
    
    [HideInInspector]public bool isCritical;
    [HideInInspector]public CharacterData_SO characterData;
    
    #region Read from Data_SO

    private void Awake()
    {
        //基础值模板赋值
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }

    public int MaxHealth
        {
            get => characterData != null ? characterData.maxHealth : 0;
            set => characterData.maxHealth = value;
        }
        public int CurrentHealth
        {
            get => characterData != null ? characterData.currentHealth : 0;
            set => characterData.currentHealth = value;
        }
        public int BaseDefence
        {
            get => characterData != null ? characterData.baseDefence : 0;
            set => characterData.baseDefence = value;
        }
        public int CurrentDefence
        {
            get => characterData != null ? characterData.currentDefence : 0;
            set => characterData.currentDefence = value;
        }

    #endregion

    #region Character Combat
    //Character to character
    public void TakeDamage(CharacterStats attacker, CharacterStats defender)
    {
        //避免产生负值
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefence, 1);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        //敌人血量UI更新
        UpdataHealthOnAttack?.Invoke(CurrentHealth,MaxHealth);
        print("GetHurt: "+defender.gameObject.name+"  Damage: "+damage+"  CurrentHealth: "+CurrentHealth);
        //经验值更新
        if (CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);
            Debug.Log("Name: " + attacker.transform.name + "  Get Exp: " + characterData.killPoint);
        }
        //暴击动画切换
        if (attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }
    }
    //character to Object
    public void TakeDamage(int damage, CharacterStats defender)
    {
        int currentDamage = Mathf.Max(damage - defender.CurrentDefence, 1);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        UpdataHealthOnAttack?.Invoke(CurrentHealth,MaxHealth);
        //回收由Object击败的经验值
        if (CurrentHealth <= 0)
        {
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
        }
        print("GetHurt: "+defender.gameObject.name+"  Damage: "+damage+"  CurrentHealth: "+CurrentHealth);
    }
    private int CurrentDamage()
    {
        float coreDamage = Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
        }

        return (int)coreDamage;
    }

    #endregion
    
}
