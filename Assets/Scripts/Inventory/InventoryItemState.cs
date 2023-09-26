using System;
using UnityEngine;

[Serializable]
public class InventoryItemState : IInventoryItemState
{
    private int _itemAmount;
    public int Amount { get => _itemAmount; set => _itemAmount = value; }
    
    public InventoryItemState()
    {
        _itemAmount = 0;
    }
}