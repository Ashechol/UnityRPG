using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    //TODO: 背包数据保存
    [Header("Inventory Data")]
    public InventoryData_SO inventoryData;
}
