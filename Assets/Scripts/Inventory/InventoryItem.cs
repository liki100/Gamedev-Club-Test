using System;
using UnityEngine;

public class InventoryItem : IInventoryItem
{
    public IInventoryItemInfo Info { get; }
    public IInventoryItemState State {get;}
    public string Id => Info.Id;
    
    public InventoryItem(IInventoryItemInfo info)
    {
        Info = info;
        State = new InventoryItemState();
    }
}