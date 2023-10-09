using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour, IDamageable, IService
{
    [SerializeField] private float _health;
    [SerializeField] private int _capacityInventory;
    
    private float _currentHealth;
    private Inventory _inventory;
    private EventBus _eventBus;

    public Inventory Inventory => _inventory;

    public void Init()
    {
        _inventory = new Inventory(_capacityInventory);
        _eventBus = ServiceLocator.Current.Get<EventBus>();
    }

    public void ApplyDamage(float damage)
    {
        _currentHealth -= damage;
        
        if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }

        _eventBus.Invoke(new CharacterHealthChangedSignal(_currentHealth/_health));

        if (_currentHealth == 0)
        {
            gameObject.SetActive(false);
            _eventBus.Invoke(new CharacterDeadSignal());
        }
    }

    public SaveManager.CharacterData GetData()
    {
        var data = new SaveManager.CharacterData()
        {
            Health = _currentHealth,
            Position = transform.position
        };
        
        return data;
    }

    public void SetData(SaveManager.CharacterData data)
    {
        transform.position = data.Position;
        _currentHealth = data.Health;
        _eventBus.Invoke(new CharacterHealthChangedSignal(_currentHealth/_health));
    }

    public void DefaultData()
    {
        _currentHealth = _health;
        _eventBus.Invoke(new CharacterHealthChangedSignal(_currentHealth/_health));
    }
}
