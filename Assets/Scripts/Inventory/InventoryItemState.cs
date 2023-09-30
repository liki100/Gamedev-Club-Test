public class InventoryItemState : IInventoryItemState
{
    public int Amount { get; set; }
    public bool isEquipped { get; set; }

    public InventoryItemState()
    {
        Amount = 0;
        isEquipped = false;
    }
}