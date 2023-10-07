public class InventorySlot : IInventorySlot
{
    public bool IsEmpty => Item == null;
    public IInventoryItem Item { get; private set; }
    public ItemType Type { get; private set; }
    public string ItemId => Item.Id;
    public int Amount => IsEmpty ? 0 : Item.Amount;

    public InventorySlot(ItemType type)
    {
        Type = type;
    }
    
    public void SetItem(IInventoryItem item)
    {
        /*if (!IsEmpty)
            return;*/

        Item = item;
    }

    public void Clear()
    {
        if (IsEmpty)
            return;
        
        Item = null;
    }

    public SaveManager.InventoryData GetData()
    {
        var data = new SaveManager.InventoryData()
        {
            InfoId = ItemId,
            Amount = Amount,
        };
        
        return data;
    }
}