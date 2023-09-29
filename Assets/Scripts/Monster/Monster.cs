using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour, IDamageable
{
    [SerializeField] private bool _facingRight;
    [SerializeField] private GameObject _skin;
    
    [SerializeField, Min(0f)] private float _health;
    [SerializeField, Min(0f)] private float _speed;
    [SerializeField] private MelleAttack _attack;
    [SerializeField] private List<Drop> _drops;

    private float _currentHealth;
    private Character _target;
    private EventBus _eventBus;

    public event Action<float> OnHealthChangedEvent;

    public void Init()
    {
        _eventBus = ServiceLocator.Current.Get<EventBus>();
        _currentHealth = _health;
        OnHealthChangedEvent?.Invoke(_currentHealth/_health);
    }
    
    private void Update()
    {
        if (_target == null)
            return;

        var position = transform.position;
        var targetPosition = _target.transform.position;

        switch (position.x-targetPosition.x)
        {
            case < 0 when !_facingRight:
            case > 0 when _facingRight:
                Flip();
                break;
        }
        
        var distanceBetweenObjects = Vector3.Distance(transform.position, targetPosition);

        if (distanceBetweenObjects > _attack.AttackRange + .25f)
        {
            transform.position = Vector3.MoveTowards(position, targetPosition, _speed * Time.deltaTime);
            return;
        }

        _attack.PerformAttack();
    }
    
    
    public void ApplyDamage(float damage)
    {
        _currentHealth -= damage;

        OnHealthChangedEvent?.Invoke(_currentHealth/_health);
        
        if (_currentHealth <= 0)
        {
            DropItem();
            _eventBus.Invoke(new MonsterDeadSignal(this));
            Destroy(gameObject);
        }
    }
    
    private void Flip()
    {
        _facingRight = !_facingRight;
        
        _skin.transform.Rotate(0f,180f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Character player))
        {
            _target = player;
        }
    }

    private void DropItem()
    {
        if (_drops.Count == 0)
            return;
        
        var rIndex = Random.Range(0, _drops.Count);
        var drop = _drops[rIndex];
        var rAmount = Random.Range(drop.AmountMin, drop.AmountMax);

        var spawnerItems = ServiceLocator.Current.Get<SpawnerItems>();

        var data = new SaveManager.ItemData()
        {
            Amount = rAmount,
            Position = transform.position,
        };
        
        var item = spawnerItems.SpawnItem();
        item.SetData(data, drop.Info);
    }

    public SaveManager.MonsterData GetData()
    {
        var data = new SaveManager.MonsterData()
        {
            Position = transform.position,
            Target = _target != null,
            Health = _currentHealth
        };

        return data;
    }

    public void SetData(SaveManager.MonsterData data)
    {
        transform.position = data.Position;
        _target = data.Target ? ServiceLocator.Current.Get<Character>() : null;
        _currentHealth = data.Health;
        
        _eventBus = ServiceLocator.Current.Get<EventBus>();
        OnHealthChangedEvent?.Invoke(_currentHealth/_health);
    }
}
