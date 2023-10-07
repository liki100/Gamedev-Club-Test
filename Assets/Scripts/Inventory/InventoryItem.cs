using UnityEngine;

public class InventoryItem : IInventoryItem
{
    public IInventoryItemInfo Info { get; }
    public int Amount { get; private set; }
    public string Id => Info.Id;

    public InventoryItem(IInventoryItemInfo info, int amount = 1)
    {
        Info = info;
        Amount = amount;
    }
    
    public void AddAmount(int value)
    {
        Amount += value;
    }
}