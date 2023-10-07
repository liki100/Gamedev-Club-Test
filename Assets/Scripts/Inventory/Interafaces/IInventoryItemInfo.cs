using UnityEngine;

public interface IInventoryItemInfo
{ 
    string Id { get; }
    ItemType Type {get;}
    Sprite SpriteIcon { get; }
    bool Stackable { get; }
    bool Equippable { get; }
}