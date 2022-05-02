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
            GameManager.Instance.playerStats.EquipWeapon(itemData);

            Destroy(gameObject);
        }
    }
}
