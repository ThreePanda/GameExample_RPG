using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO CharacterData;
    public AttackData_SO attackData;
    [HideInInspector] public bool isCritical;
    #region Read from Data_SO
    
    public int MaxHealth
        {
            get => CharacterData != null ? CharacterData.maxHealth : 0;
            set => CharacterData.maxHealth = value;
        }
        public int CurrentHealth
        {
            get => CharacterData != null ? CharacterData.currentHealth : 0;
            set => CharacterData.currentHealth = value;
        }
        public int BaseDefence
        {
            get => CharacterData != null ? CharacterData.baseDefence : 0;
            set => CharacterData.baseDefence = value;
        }
        public int CurrentDefence
        {
            get => CharacterData != null ? CharacterData.currentDefence : 0;
            set => CharacterData.currentDefence = value;
        }

    #endregion

    #region Character Combat

     public void TakeDamage(CharacterStats attacker, CharacterStats defender)
    {
        //避免产生负值
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        Debug.Log("GetHurt:"+defender.gameObject.tag+"  Damage:"+damage+"  CurrentHealth:"+CurrentHealth);
        if (attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }
        //TODO:人物UI，经验收集
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
