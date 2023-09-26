using UnityEngine;

public interface IInventoryItemInfo
{ 
    string Id { get; }
    string Title { get; }
    Sprite SpriteIcon { get; }
    bool Stackable { get; }
}