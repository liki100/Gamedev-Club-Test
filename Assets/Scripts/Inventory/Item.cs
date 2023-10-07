using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _skin; 
    
    private DefaultItemInfo _info;
    private int _amount;
    private EventBus _eventBus;

    public void Init()
    {
        _eventBus = ServiceLocator.Current.Get<EventBus>();
        _skin.sprite = _info.SpriteIcon;
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Character character))
        {
            var item = new InventoryItem(_info, _amount);

            if (!character.Inventory.TryAdd(item)) 
                return;
            
            _eventBus.Invoke(new ItemRaisedSignal(this));
            Destroy(gameObject);
        }
    }

    public SaveManager.ItemData GetData()
    {
        var data = new SaveManager.ItemData()
        {
            InfoId = _info.Id,
            Amount = _amount,
            Position = transform.position
        };

        return data;
    }

    public void SetData(SaveManager.ItemData data, DefaultItemInfo info)
    {
        _info = info;
        _amount = data.Amount;
        transform.position = data.Position;
    }
}
