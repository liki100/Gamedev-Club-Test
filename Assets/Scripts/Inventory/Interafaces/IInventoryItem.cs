public interface IInventoryItem
{
    public IInventoryItemInfo Info { get; }
    public int Amount { get; }
    public string Id { get; }

    public void AddAmount(int value);
}