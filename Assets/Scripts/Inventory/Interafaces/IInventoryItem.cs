public interface IInventoryItem
{
    IInventoryItemInfo Info { get; }
    IInventoryItemState State { get; }

    string Id { get; }
}