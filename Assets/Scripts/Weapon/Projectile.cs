using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Projectile : MonoBehaviour
{
    [Header("Common")] 
    [SerializeField, Min(0f)] private float _damage = 10f;
    [SerializeField] private ProjectileDisposeType _disposeType = ProjectileDisposeType.OnAnyCollision;

    [Header("Rigidbody")] 
    [SerializeField] private Rigidbody2D _projectileRigidbody;

    [Header("Effect On Destroy")] 
    [SerializeField] private bool _spawnEffectOnDestroy = true;
    [SerializeField] private ParticleSystem _effectOnDestroyTemplate;
    [SerializeField, Min(0)] private float _effectOnDestroyLifetime = 2f;

    private bool _isProjectileDisposed;

    protected float Damage => _damage;
    public bool IsProjectileDisposed => _isProjectileDisposed;
    public Rigidbody2D Rigidbody => _projectileRigidbody;

    public void Init(float damage)
    {
        _damage += damage;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isProjectileDisposed)
            return;

        if (collision.gameObject.TryGetComponent(out IDamageable damageable))
        {
            OnTargetCollision(collision, damageable);

            if (_disposeType == ProjectileDisposeType.OnTargetCollision)
            {
                DisposeProjectile();
            }
        }
        else
        {
            OnOtherCollision(collision);
        }

        OnAnyCollision(collision);

        if (_disposeType == ProjectileDisposeType.OnAnyCollision)
        {
            DisposeProjectile();
        }
    }
    
    public void DisposeProjectile()
    {
        OnProjectileDispose();

        SpawnEffectOnDestroy();
        
        Destroy(gameObject);

        _isProjectileDisposed = true;
    }

    private void SpawnEffectOnDestroy()
    {
        if (_spawnEffectOnDestroy == false)
            return;

        var effect = Instantiate(_effectOnDestroyTemplate, transform.position, Quaternion.identity);

        Destroy(effect.gameObject, _effectOnDestroyLifetime);
    }

    protected virtual void OnProjectileDispose() { }

    protected virtual void OnTargetCollision(Collision2D collision, IDamageable damageable) { }
    
    protected virtual void OnOtherCollision(Collision2D collision) { }

    protected virtual void OnAnyCollision(Collision2D collision) { }
}
