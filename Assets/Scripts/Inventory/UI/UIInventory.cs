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
    [SerializeField] private Button _equipButton;

    [SerializeField] private UIInventorySlot _uiWeaponSlot;
    
    public List<UIInventorySlot> _uiSlots;
    private Inventory _inventory;

    public void Init()
    {
        _inventory = ServiceLocator.Current.Get<Character>().Inventory;
        _inventory.OnInventoryStateChangedEvent += OnInventoryStateChanged;

        _uiSlots = new List<UIInventorySlot>();

        CreateUISlot();
    }

    private void CreateUISlot()
    {
        for (var i = 0; i < _inventory.Capacity; i++)
        {
            _uiSlots.Add(Instantiate(_slotTemplate, _container));
        }
        
        var allSlots = _inventory.GetAllSlot();
        for (var i = 0; i < allSlots.Length; i++)
        {
            var index = i;
            var slot = allSlots[index];
            var uiSlot = _uiSlots[index];
            uiSlot.SetSlot(slot);
            uiSlot.Button.onClick.AddListener(() => OnSelected(index));
            uiSlot.Refresh();
        }
        _uiWeaponSlot.SetSlot(_inventory.GetSlotWithType(ItemType.Weapon));
        _uiWeaponSlot.Refresh();
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
        _uiWeaponSlot.Refresh();
        _inventoryCountText.text = $"{_inventory.GetAllSlotIsNotEmpty().Length}/{_inventory.Capacity}";
        _deleteButton.interactable = false;
        _equipButton.interactable = false;
    }

    private void OnSelected(int index)
    {
        _deleteButton.interactable = false;
        _equipButton.interactable = false;
        
        _equipButton.onClick.RemoveAllListeners();
        _deleteButton.onClick.RemoveAllListeners();
        
        var slots = _inventory.GetAllSlot();
        if (slots[index].Item.Info.Equippable)
        {
            _equipButton.interactable = true;
            _equipButton.onClick.AddListener(() => OnEquipClick(index));
        }
        
        _deleteButton.interactable = true;
        _deleteButton.onClick.AddListener(() => OnDeleteClick(index));
    }

    private void OnDeleteClick(int index)
    {
        _inventory.Remove(index);
        _deleteButton.onClick.RemoveAllListeners();
        OnInventoryStateChanged();
    }

    private void OnEquipClick(int index)
    {
        _inventory.EquipItem(index);
        _equipButton.onClick.RemoveAllListeners();
        OnInventoryStateChanged();
    }
}