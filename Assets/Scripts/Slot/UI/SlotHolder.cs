using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotType { Bag, Weapon, Armor, Action }
public class SlotHolder : MonoBehaviour
{
    public ItemUI itemUI;
    public SlotType slotType;

    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.Bag:
                itemUI.Bag = SlotUIManager.Instance.bagData;
                break;
            case SlotType.Weapon:
                break;
            case SlotType.Armor:
                break;
            case SlotType.Action:
                break;
        }

        var item = itemUI.Bag.items[itemUI.Index];
        itemUI.SetUpItemUI(item.itemData, item.amount);
    }
}
