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
            var index = i;
            var slot = allSlots[index];
            var uiSlot = _uiSlots[index];
            uiSlot.SetSlot(slot);
            uiSlot.Button.onClick.AddListener(() => OnSelected(index));
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

    private void OnSelected(int index)
    {
        _deleteButton.interactable = true;
        _deleteButton.onClick.AddListener(() => OnDeleteClick(index));
    }

    private void OnDeleteClick(int index)
    {
        _inventory.Remove(index);
        _deleteButton.onClick.RemoveAllListeners();
        OnInventoryStateChanged();
    }
}