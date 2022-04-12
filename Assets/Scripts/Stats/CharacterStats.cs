using System.Collections;
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

    public int Damage
    {
        get
        {
            float coreDamage = Random.Range(attackData.minDamage,
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
            float coreDamage = Random.Range(attackData.skillMinDamage,
                                  attackData.skillMaxDamage);

            if (isCritical)
                coreDamage *= attackData.criticalMultiplier;

            return (int)coreDamage;
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

    //TODO: 代码内部结构优化
    // 伤害计算
    public void TakeDamage(CharacterStats attacker)
    {
        int damage = Mathf.Max(attacker.Damage - CurrentDefence, 1);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        bool isdead = (CurrentHealth == 0);

        if (CompareTag("Enemy"))
        {
            //FIXME: 只在第一被偷袭看向玩家
            GetComponent<Transform>().LookAt(attacker.GetComponent<Transform>());

            // if (!GetComponentInChildren<VisualEffect>().enabled)
            //     GetComponentInChildren<VisualEffect>().enabled = true;
            // GetComponentInChildren<VisualEffect>().Play();
        }

        if (isdead)
        {
            if (CompareTag("Player"))
            {
                GetComponent<PlayerController>().IsDead = isdead;
                GameManager.Instance.NotifyObservers();
            }

            if (CompareTag("Enemy"))
            {
                GetComponent<EnemyController>().IsDead = isdead;
            }
        }

        if (attacker.isCritical)  // 攻击者暴击
        {
            GetComponent<Animator>().SetTrigger("hit");
        }

        //TODO: Update UI
        //TODO: Level Up
    }

    public void TakeDamage(int damage)
    {
        damage = Mathf.Max(damage - CurrentDefence, 1);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        bool isdead = (CurrentHealth == 0);

        if (isdead)
        {
            if (CompareTag("Player"))
            {
                GetComponent<PlayerController>().IsDead = isdead;
                GameManager.Instance.NotifyObservers();
            }

            if (CompareTag("Enemy"))
            {
                GetComponent<EnemyController>().IsDead = isdead;
            }
        }
    }

    // 使用属性即刻更简洁实现下列功能
    // private int CurrentDamage()
    // {
    //     float coreDamage = Random.Range(attackData.minDamage, attackData.maxDamage);

    //     if (isCritical)
    //     {
    //         coreDamage *= attackData.criticalMultiplier;
    //         // Debug.Log("Critical Attack!" + coreDamage);
    //     }

    //     return (int)coreDamage;
    // }

    #endregion

}
