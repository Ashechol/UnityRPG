using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "RPG/Inventory/Inventory Data")]
public class InventoryData_SO : ScriptableObject
{
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(ItemData_SO itemData, int amount)
    {
        bool found = false;

        if (itemData.stackable)
        {
            foreach (var item in items)
            {
                if (item.itemData == itemData)
                {
                    item.amount += amount;
                    found = true;
                    break;
                }
            }
        }

        if (!found)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemData == null)
                {
                    items[i].itemData = itemData;
                    items[i].amount = amount;
                    break;
                }
            }
        }
    }
}

// 序列化类才能看见
[System.Serializable]
public class InventoryItem
{
    public ItemData_SO itemData;
    public int amount;
}
