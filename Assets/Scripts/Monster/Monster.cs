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
    
    public float Health => _currentHealth;
    public Character Target => _target;
    
    public event Action<float> OnHealthChangedEvent;
    public event Action<Monster> OnDiedEvent;

    public void Init()
    {
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
            OnDiedEvent?.Invoke(this);
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
        var item = Instantiate(drop.Info.ItemTemplate, transform.position, Quaternion.identity);
        item.SetInfo(drop.Info);
        item.SetAmount(rAmount);
    }

    public void SetData(SaveManager.MonsterData data, Character target)
    {
        _currentHealth = data.Health;
        _target = target;
        OnHealthChangedEvent?.Invoke(_currentHealth/_health);
    }
}
