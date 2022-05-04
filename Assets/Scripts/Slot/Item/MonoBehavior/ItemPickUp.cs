using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SlotUIManager.Instance.bagData.AddItem(itemData, itemData.itemAmount);
            SlotUIManager.Instance.bagContainerUI.RefreshUI();
            Destroy(gameObject);
        }
    }
}
