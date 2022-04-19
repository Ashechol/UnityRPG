using System;
using UnityEngine;
using UnityEngine.VFX;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO templeteData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

    #region Read from Data_SO
    public int MaxHealth // properties
    {
        get
        {
            if (characterData != null)
                return characterData.maxHealth;
            else
                return 0;
        }
        set
        {
            characterData.maxHealth = value;
        }
    }

    public int CurrentHealth // properties
    {
        get
        {
            if (characterData != null)
                return characterData.currentHealth;
            else
                return 0;
        }
        set
        {
            characterData.currentHealth = value;
        }
    }

    public int BaseDefence // properties
    {
        get
        {
            if (characterData != null)
                return characterData.baseDefence;
            else
                return 0;
        }
        set
        {
            characterData.baseDefence = value;
        }
    }

    public int CurrentDefence // properties
    {
        get
        {
            if (characterData != null)
                return characterData.currentDefence;
            else
                return 0;
        }
        set
        {
            characterData.currentDefence = value;
        }
    }

    public int Damage
    {
        get
        {
            float coreDamage = UnityEngine.Random.Range(attackData.minDamage,
                                  attackData.maxDamage);
            if (isCritical)
                coreDamage *= attackData.criticalMultiplier;

            return (int)coreDamage;
        }
    }

    public int SkillDamage
    {
        get
        {
            float coreDamage = UnityEngine.Random.Range(attackData.skillMinDamage,
                                  attackData.skillMaxDamage);

            if (isCritical)
                coreDamage *= attackData.criticalMultiplier;

            return (int)coreDamage;
        }
    }

    #endregion

    protected virtual void Awake()
    {
        if (templeteData != null)
        {
            characterData = Instantiate(templeteData);
        }
    }

    public virtual void TakeDamage(int damage, bool isCritical = false)
    {
        damage = Mathf.Max(damage - CurrentDefence, 1);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        bool isdead = (CurrentHealth == 0);

        if (isdead)
        {
            if (CompareTag("Player"))
            {
                if (isdead)
                {
                    GetComponent<PlayerController>().IsDead = isdead;
                    GameManager.Instance.NotifyObservers();
                }
            }

            if (CompareTag("Enemy"))
            {
                GetComponent<EnemyController>().IsDead = isdead;
            }
        }
        else if (isCritical)
            GetComponent<Animator>().SetTrigger("hit");
    }
}
