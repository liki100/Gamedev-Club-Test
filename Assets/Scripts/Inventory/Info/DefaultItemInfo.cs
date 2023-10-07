using UnityEngine;

[CreateAssetMenu(menuName = "Data/Default Info", fileName = "_info")]
public class DefaultItemInfo : ScriptableObject, IInventoryItemInfo
{
    [SerializeField] private string _id;
    [SerializeField] private ItemType _type = ItemType.Default;
    [SerializeField] private Sprite _spriteIcon;
    [SerializeField] private bool _stackable;
    [SerializeField] private bool _equippable;
    
    public string Id => _id;
    public ItemType Type => _type;
    public Sprite SpriteIcon => _spriteIcon;
    public bool Stackable => _stackable;
    public bool Equippable => _equippable;
}