using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour, IDamageable, IService
{
    [SerializeField] private float _health;
    [SerializeField] private int _capacityInventory;
    private int _overlapResultsCount;
    
    private float _currentHealth;
    private Rigidbody2D _rigidbody;
    private Vector3 _moveDirection;
    private Inventory _inventory;
    
    private EventBus _eventBus;

    public Inventory Inventory => _inventory;
    public float Health => _currentHealth;

    public void Init()
    {
        gameObject.SetActive(true);
        if (_currentHealth == 0)
        {
            _currentHealth = _health;
        }
        _inventory = new Inventory(_capacityInventory);
        _eventBus = ServiceLocator.Current.Get<EventBus>();
        _eventBus.Invoke(new HealthChangedSignal(_currentHealth/_health));
    }

    public void ApplyDamage(float damage)
    {
        _currentHealth -= damage;
        
        if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }

        _eventBus.Invoke(new HealthChangedSignal(_currentHealth/_health));

        if (_currentHealth == 0)
        {
            gameObject.SetActive(false);
            _eventBus.Invoke(new PlayerDeadSignal());
        }
    }

    public void SetHealth(float health)
    {
        _currentHealth = health;
    }
}
