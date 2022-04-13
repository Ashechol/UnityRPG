using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "RPG/Enemy Data")]
public class EnemyData_SO : CharacterData_SO
{
    [Header("Advance Info")]
    public int dropExp;
    // public int level;
}
