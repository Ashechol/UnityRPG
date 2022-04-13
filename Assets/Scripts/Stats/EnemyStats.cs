using System;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    [HideInInspector]
    public EnemyData_SO enemyData;
    public event Action<int, int> OnHealthBarUpdate;

    protected override void Awake()
    {
        base.Awake();
        enemyData = characterData as EnemyData_SO;
        Debug.Log(CurrentHealth);
    }

    public void TakeDamage(PlayerStats attacker)
    {
        int damage = Mathf.Max(attacker.Damage - CurrentDefence, 1);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        bool isdead = (CurrentHealth == 0);

        //FIXME: 只在第一被偷袭看向玩家
        GetComponent<Transform>().LookAt(attacker.GetComponent<Transform>());

        // if (!GetComponentInChildren<VisualEffect>().enabled)
        //     GetComponentInChildren<VisualEffect>().enabled = true;
        // GetComponentInChildren<VisualEffect>().Play();

        if (isdead)
            GetComponent<EnemyController>().IsDead = isdead;

        if (attacker.isCritical)  // 攻击者暴击
            GetComponent<Animator>().SetTrigger("hit");

        // Update UI
        OnHealthBarUpdate?.Invoke(CurrentHealth, MaxHealth);

        //TODO: Level Up
        if (CurrentHealth <= 0)
        {
            attacker.UpdateExp(enemyData.dropExp);
        }
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

        OnHealthBarUpdate?.Invoke(CurrentHealth, MaxHealth);
    }
}
