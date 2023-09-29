using System;

public interface IInventorySlot
{
    bool IsEmpty { get; }
    
    IInventoryItem Item { get; }
    string ItemId { get; }
    int Amount { get; }

    void SetItem(IInventoryItem item);
    void Clear();

    SaveManager.InventoryData GetData();
}