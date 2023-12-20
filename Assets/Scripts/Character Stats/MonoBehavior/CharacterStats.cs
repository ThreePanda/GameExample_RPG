using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour
{
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
        print("GetHurt:"+defender.gameObject.tag+"  Damage:"+damage+"  CurrentHealth:"+CurrentHealth);
        if (attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }
        //TODO:人物UI，经验收集
    }
    //Object to character
    public void TakeDamage(int damage, CharacterStats defender)
    {
        int currentDamage = Mathf.Max(damage - defender.CurrentDefence, 1);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        print("GetHurt:"+defender.gameObject.tag+"  Damage:"+damage+"  CurrentHealth:"+CurrentHealth);
    }
    private int CurrentDamage()
    {
        float coreDamage = Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("Critical!" + coreDamage);
        }

        return (int)coreDamage;
    }

    #endregion
    
}
