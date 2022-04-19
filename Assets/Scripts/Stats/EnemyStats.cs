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
    }

    public override void TakeDamage(int damage, bool isCritical = false)
    {
        base.TakeDamage(damage, isCritical);

        GetComponent<EnemyController>().Alarm();

        OnHealthBarUpdate?.Invoke(CurrentHealth, MaxHealth);
    }
}
