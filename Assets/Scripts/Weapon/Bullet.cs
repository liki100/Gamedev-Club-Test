using UnityEngine;

public class Bullet : Projectile
{
    protected override void OnTargetCollision(Collision2D collision, IDamageable damageable)
    {
        damageable.ApplyDamage(Damage);
    }
}
