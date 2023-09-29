using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerItems : MonoBehaviour, IService
{
    [SerializeField] private Item _itemTemplate;

    private List<Item> _items;
    private EventBus _eventBus;
    
    public List<Item> Items => _items;
    
    public void Init()
    {
        _eventBus = ServiceLocator.Current.Get<EventBus>();
        _eventBus.Subscribe<ItemRaisedSignal>(ItemRaised);
        _items = new List<Item>();
    }

    public Item SpawnItem()
    {
        var item = Instantiate(_itemTemplate);
        _items.Add(item);

        return item;
    }
    
    public void DeleteItems()
    {
        foreach (var item in _items)
            Destroy(item.gameObject);
        
        _items.Clear();
    }
    
    private void ItemRaised(ItemRaisedSignal signal)
    {
        _items.Remove(signal.Value);
    }

    private void OnDestroy()
    {
        _eventBus.Unsubscribe<ItemRaisedSignal>(ItemRaised);
    }
}
