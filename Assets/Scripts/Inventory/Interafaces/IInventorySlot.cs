public interface IInventorySlot
{
    bool IsEmpty { get; }
    
    IInventoryItem Item { get; }
    ItemType Type { get; }
    string ItemId { get; }
    int Amount { get; }

    void SetItem(IInventoryItem item);
    void Clear();

    SaveManager.InventoryData GetData();
}