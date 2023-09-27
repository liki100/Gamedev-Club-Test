using System.Collections.Generic;
using UnityEngine;

public class SpawnerItems : MonoBehaviour, IService
{
    [SerializeField] private Item _itemTemplate;

    private List<Item> _items;
    
    public List<Item> Items => _items;
    
    public void Init()
    {
        _items = new List<Item>();
    }

    public void SpawnItem(SaveManager.ItemData data, InventoryItemInfo info)
    {
        var item = Instantiate(_itemTemplate, data.Position, Quaternion.identity);
        item.SetInfo(info);
        item.SetAmount(data.Amount);
        item.OnRaisedEvent += OnRaised;
        _items.Add(item);
    }
    
    public void DeleteItems()
    {
        foreach (var item in _items)
        {
            item.OnRaisedEvent -= OnRaised;
            Destroy(item.gameObject);
        }
        _items.Clear();
    }
    
    private void OnRaised(Item item)
    {
        item.OnRaisedEvent -= OnRaised;
        _items.Remove(item);
    }
}
