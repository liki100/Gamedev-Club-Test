using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : IInventory
{
    public Action OnInventoryStateChangedEvent;

    public int Capacity { get; set; }
    public bool IsFull => _slots.All(slot => !slot.IsEmpty);

    private List<IInventorySlot> _slots;

    public Inventory(int capacity)
    {
        Capacity = capacity;

        _slots = new List<IInventorySlot>(Capacity);

        for (var i = 0; i < Capacity; i++)
        {
            _slots.Add(new InventorySlot());
        }
    }
    
    public IInventoryItem GetItem(string itemId)
    {
        return _slots.Find(slot => slot.ItemId == itemId).Item;
    }

    public IInventoryItem[] GetAllItems()
    {
        var allItems = new List<IInventoryItem>();
        foreach (var slot in _slots)
        {
            if (!slot.IsEmpty)
            {
                allItems.Add(slot.Item);
            }
        }
        
        return allItems.ToArray();
    }

    public bool TryAdd(IInventoryItem item)
    {
        var slotWitchSameItemButNotEmpty = _slots.Find(slot => !slot.IsEmpty && slot.ItemId == item.Id && item.Info.Stackable);

        if (slotWitchSameItemButNotEmpty != null)
            return TryToAddToSlot(slotWitchSameItemButNotEmpty, item);

        var emptySlot = _slots.Find(slot => slot.IsEmpty);

        if (emptySlot != null)
            return TryToAddToSlot(emptySlot, item);

        return false;
    }

    private bool TryToAddToSlot(IInventorySlot slot, IInventoryItem item)
    {
        if (slot.IsEmpty)
        {
            slot.SetItem(item);
        }
        else
        {
            slot.Item.State.Amount += item.State.Amount;
        }
        
        OnInventoryStateChangedEvent?.Invoke();

        return true;
    }
    
    public void Remove(int index)
    {
        var slot = _slots[index];

        if (slot == null)
            return;
        
        slot.Clear();
        OnInventoryStateChangedEvent?.Invoke();
    }

    public void Clear()
    {
        var slotsIsNotEmpty = GetAllSlotIsNotEmpty();
        foreach (var slot in slotsIsNotEmpty)
        {
            slot.Clear();
        }
    }

    public bool HasItem(string itemId, out IInventoryItem item)
    {
        item = GetItem(itemId);
        return item != null;
    }

    public IInventorySlot[] GetAllSlot()
    {
        return _slots.ToArray();
    }

    public IInventorySlot[] GetAllSlotIsNotEmpty()
    {
        return _slots.FindAll(slot => !slot.IsEmpty).ToArray();
    }

    private IInventorySlot GetSlot(string itemId)
    {
        return _slots.Find(slot => !slot.IsEmpty && slot.ItemId == itemId);
    }
}