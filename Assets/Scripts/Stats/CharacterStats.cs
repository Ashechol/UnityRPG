using System.Collections;
using UnityEngine;

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
                return characterData.MaxHealth;
            else
                return 0;
        }
        set
        {
            characterData.MaxHealth = value;
        }
    }

    public int CurrentHealth // properties
    {
        get
        {
            if (characterData != null)
                return characterData.CurrentHealth;
            else
                return 0;
        }
        set
        {
            characterData.CurrentHealth = value;
        }
    }

    public int BaseDefence // properties
    {
        get
        {
            if (characterData != null)
                return characterData.BaseDefence;
            else
                return 0;
        }
        set
        {
            characterData.BaseDefence = value;
        }
    }

    public int CurrentDefence // properties
    {
        get
        {
            if (characterData != null)
                return characterData.CurrentDefence;
            else
                return 0;
        }
        set
        {
            characterData.CurrentDefence = value;
        }
    }

    #endregion

    void Awake()
    {
        if (templeteData != null)
        {
            characterData = Instantiate(templeteData);
        }
    }

    #region Character Combat

    public void TakeDamage(CharacterStats attacker, CharacterStats defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 1);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        bool isdead = (CurrentHealth == 0);
        if (isdead)
        {
            if (CompareTag("Player"))
            {
                GetComponentInParent<PlayerController>().IsDead = isdead;
                GameManager.Instance.NotifyObservers();
            }

            if (CompareTag("Enemy"))
            {
                GetComponentInParent<EnemyController>().IsDead = isdead;
            }
        }

        if (attacker.isCritical)  // 攻击者暴击
        {
            defener.GetComponent<Animator>().SetTrigger("hit");
        }
        //TODO: Update UI
        //TODO: Level Up
    }

    private int CurrentDamage()
    {
        float coreDamage = Random.Range(attackData.minDamage, attackData.maxDamage);

        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            // Debug.Log("Critical Attack!" + coreDamage);
        }

        return (int)coreDamage;
    }

    #endregion

}
