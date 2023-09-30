using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Create New ItemInfo", fileName = "InventoryItemInfo")]
public class InventoryItemInfo : ScriptableObject, IInventoryItemInfo
{
    [SerializeField] private string _id;
    [SerializeField] private string _title;
    [SerializeField] private ItemType _type = ItemType.Default;
    [SerializeField] private Sprite _spriteIcon;
    [SerializeField] private bool _stackable;
    [SerializeField] private bool _equippable;

    
    public string Id => _id;
    public string Title => _title;
    public ItemType Type => _type;
    public Sprite SpriteIcon => _spriteIcon;
    public bool Stackable => _stackable;
    public bool Equippable => _equippable;
    
    public virtual string Display()
    {
        return "";
    }
}