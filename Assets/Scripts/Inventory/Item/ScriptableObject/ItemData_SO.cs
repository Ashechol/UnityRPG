using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Usable, Weapon, Armor }

[CreateAssetMenu(fileName = "New Item", menuName = "RPG/Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int itemAmount;
    public bool stackable;
    [TextArea] public string description = "";

    [Header("Weapon")]
    public GameObject weaponPrefab;
    public float attackRange;
    public float attackRate;
    public int minDamage;
    public int maxDamage;
    public float criticalMultiplier;
    public float criticalChance;
    public int minLevel;
}
