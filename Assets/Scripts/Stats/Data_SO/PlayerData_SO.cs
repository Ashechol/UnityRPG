using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "RPG/Player Data")]
public class PlayerData_SO : CharacterData_SO
{
    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int nextLevelExp;
    public int currentExp;
    public float levelBuff;
    public float healthBuff;

    // public float LevelMultiplier
    // {
    //     get { return 1 + (currentLevel - 1) * levelBuff; }
    // }
}
