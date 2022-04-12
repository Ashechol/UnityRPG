using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "RPG/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    [Header("Base Info")]
    public float attackRange;
    public float skillRange;
    public float coolDown;
    public int minDamage;
    public int maxDamage;
    public int skillMinDamage;
    public int skillMaxDamage;
    public float criticalMultiplier;
    public float criticalChance;
}
