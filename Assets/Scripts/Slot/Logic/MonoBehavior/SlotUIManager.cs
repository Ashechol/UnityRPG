using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotUIManager : Singleton<SlotUIManager>
{
    //TODO: 背包数据保存
    [Header("Inventory Data")]
    public InventoryData_SO bagData;

    [Header("Containers")]
    public ContainerUI bagContainerUI;

    void Start()
    {
        bagContainerUI.RefreshUI();
    }
}
