using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory
{
    private readonly int _capacity;
    private readonly List<IInventorySlot> _slots;
    private readonly List<IInventorySlot> _equipSlots;
    
    public int Capacity => _capacity;
    
    public Action OnInventoryStateChangedEvent;

    public Inventory(int capacity)
    {
        _capacity = capacity;

        _slots = new List<IInventorySlot>(_capacity);

        for (var i = 0; i < _capacity; i++)
        {
            _slots.Add(new InventorySlot(ItemType.Default));
        }

        _equipSlots = new List<IInventorySlot>()
        {
            new InventorySlot(ItemType.Weapon),
            new InventorySlot(ItemType.Hat)
        };
    }
    
    public bool TryAdd(IInventoryItem item)
    {
        var slotWithSameItemButNotEmpty = _slots.Find(slot => !slot.IsEmpty && slot.ItemId == item.Id && item.Info.Stackable);

        if (slotWithSameItemButNotEmpty != null)
            return TryToAddToSlot(slotWithSameItemButNotEmpty, item);

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
            slot.Item.AddAmount(item.Amount);
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

    public void EquipItem(int index)
    {
        var slot = _slots[index];
        
        if (slot == null)
            return;

        var item = slot.Item;

        var equippable = item.Info.Equippable;

        if (!equippable)
            return;

        var slotWithSameItemType = _equipSlots.Find(equipSlot => equipSlot.Type == item.Info.Type);
        
        if (slotWithSameItemType != null)
            TryToEquip(slot, slotWithSameItemType);
    }
    
    private void TryToEquip(IInventorySlot slot, IInventorySlot equipSlot)
    {
        if (equipSlot.IsEmpty)
        {
            equipSlot.SetItem(slot.Item);
            slot.Clear();
        }
        else
        {
            var currentItem = equipSlot.Item;
            equipSlot.SetItem(slot.Item);
            slot.SetItem(currentItem);
        }
        
        var weaponRange = ServiceLocator.Current.Get<RangeWeapon>();
        weaponRange.UpdateData();
        
        OnInventoryStateChangedEvent?.Invoke();
    }

    public IInventorySlot GetSlotWithType(ItemType type)
    {
        return _equipSlots.Find(equipSlot => equipSlot.Type == type);
    }

    public IInventorySlot[] GetAllSlot()
    {
        return _slots.ToArray();
    }

    public IInventorySlot[] GetAllSlotIsNotEmpty()
    {
        return _slots.FindAll(slot => !slot.IsEmpty).ToArray();
    }
}