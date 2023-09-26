using UnityEngine;

public class ProjectileDisposeTimer : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _delayDispose = 5f;

    private Projectile _projectile;
    private float _currentTime;

    private void Awake()
    {
        _projectile = GetComponent<Projectile>();
    }

    private void Update()
    {
        if (_projectile.IsProjectileDisposed) 
            return;
        
        _currentTime += Time.deltaTime;

        if (_currentTime >= _delayDispose)
        {
            _projectile.DisposeProjectile();
        }
    }
}
