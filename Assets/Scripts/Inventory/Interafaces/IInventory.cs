using System;

public interface IInventory
{
    int Capacity { get; set; }
    bool IsFull { get; }

    IInventoryItem GetItem(string itemId);
    IInventoryItem[] GetAllItems();

    bool TryAdd(IInventoryItem item);
    void Remove(string itemId);
    bool HasItem(string itemId, out IInventoryItem item);
}