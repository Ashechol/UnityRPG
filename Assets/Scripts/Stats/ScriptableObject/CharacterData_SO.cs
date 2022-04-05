using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "RPG/Character Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("State Info")]
    public int MaxHealth;
    public int CurrentHealth;
    public int BaseDefence;
    public int CurrentDefence;
}
