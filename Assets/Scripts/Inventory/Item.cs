using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _skin; 
    
    private InventoryItemInfo _info;
    private int _amount;

    public InventoryItemInfo Info => _info;
    public int Amount => _amount;
    
    public event Action<Item> OnRaisedEvent;

    private void Start()
    {
        _skin.sprite = _info.SpriteIcon;
    }

    public void SetInfo(InventoryItemInfo info)
    {
        _info = info;
    }

    public void SetAmount(int amount)
    {
        _amount = !_info.Stackable ? 1 : amount;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Character character))
        {
            if (character.Inventory.IsFull)
                return;
            
            var item = new InventoryItem(_info);
            item.State.Amount = _amount;
            character.Inventory.TryAdd(item);

            OnRaisedEvent?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
