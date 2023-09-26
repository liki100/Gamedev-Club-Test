using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour, IService
{
    [SerializeField] private UIInventorySlot _slotTemplate;
    [SerializeField] private Transform _container;
    [SerializeField] private TMP_Text _inventoryCountText;
    [SerializeField] private Button _deleteButton;
    
    public List<UIInventorySlot> _uiSlots;
    private Inventory _inventory;

    public void Init()
    {
        _inventory = ServiceLocator.Current.Get<Character>().Inventory;
        
        _uiSlots = new List<UIInventorySlot>();
        
        for (var i = 0; i < _inventory.Capacity; i++)
        {
            _uiSlots.Add(Instantiate(_slotTemplate, _container));
        }
        
        _inventory.OnInventoryStateChangedEvent += OnInventoryStateChanged;

        var allSlots = _inventory.GetAllSlot();
        var allSlotsCount = allSlots.Length;
        for (var i = 0; i < allSlotsCount; i++)
        {
            var slot = allSlots[i];
            var uiSlot = _uiSlots[i];
            uiSlot.SetSlot(slot);
            uiSlot.Button.onClick.AddListener(() => OnSelected(uiSlot));
            uiSlot.Refresh();
        }
    }

    public void Clear()
    {
        foreach (var slot in _uiSlots)
        {
            slot.Button.onClick.RemoveAllListeners();
            Destroy(slot.gameObject);
        }
        _uiSlots.Clear();
    }

    private void OnEnable()
    {
        OnInventoryStateChanged();
    }

    private void OnInventoryStateChanged()
    {
        foreach (var slot in _uiSlots)
        {
            slot.Refresh();
        }
        _inventoryCountText.text = $"{_inventory.GetAllSlotIsNotEmpty().Length}/{_inventory.Capacity}";
        _deleteButton.interactable = false;
    }
    
    private void OnSelected(UIInventorySlot uiSlot)
    {
        _deleteButton.interactable = true;
        var id = uiSlot.Slot.ItemId;
        _deleteButton.onClick.AddListener(() => OnDeleteClick(id));
    }

    private void OnDeleteClick(string id)
    {
        _inventory.Remove(id);
        _deleteButton.onClick.RemoveAllListeners();
        OnInventoryStateChanged();
    }
}