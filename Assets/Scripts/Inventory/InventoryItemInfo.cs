using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Create New ItemInfo", fileName = "InventoryItemInfo")]
public class InventoryItemInfo : ScriptableObject, IInventoryItemInfo
{
    [SerializeField] private string _id;
    [SerializeField] private string _title;
    [SerializeField] private Sprite _spriteIcon;
    [SerializeField] private bool _stackable;
    [SerializeField] private Item _itemTemplate;
    
    public string Id => _id;
    public string Title => _title;
    public Sprite SpriteIcon => _spriteIcon;
    public bool Stackable => _stackable;
    public Item ItemTemplate => _itemTemplate;
}