using System;
using UnityEngine;

public class MelleAttack : MonoBehaviour
{
    [Header("Common")] 
    [SerializeField, Min(0)] private float _damage;
    [SerializeField, Min(0)] private float _cooldownTime;

    [Header("Masks")] 
    [SerializeField] private LayerMask _searchLayerMask;
    [SerializeField] private LayerMask _obstacleLayerMask;
    
    [Header("Overlap Area")] 
    [SerializeField] private Transform _overlapStartPoint;
    [SerializeField] private Vector3 _offset;
    [SerializeField, Min(0)] private float _circleRadius = 1f;
    
    [Header("Obstacles")] 
    [SerializeField] private bool _considerObstacles;
    
    [Header("Gizmos")] 
    [SerializeField] private DrawGizmosType drawGizmosType; 
    [SerializeField] private Color _gizmoColor = Color.cyan;
    
    private readonly Collider2D[] _overlapResults = new Collider2D[32];
    private int _overlapResultsCount;
    private float _currentCooldownTime;

    public float AttackRange => Mathf.Abs(_overlapStartPoint.localPosition.x + _offset.x) + _circleRadius;

    private void Update()
    {
        _currentCooldownTime -= Time.deltaTime;
    }

    [ContextMenu(nameof(PerformAttack))]
    public void PerformAttack()
    {
        if (_currentCooldownTime > 0)
            return;
        
        if (TryFindEnemies())
        {
            TryAttackEnemies();
            _currentCooldownTime = _cooldownTime;
        }
    }

    private bool TryFindEnemies()
    {
        var position = _overlapStartPoint.TransformPoint(_offset);

        _overlapResultsCount = OverlapCircle(position);

        return _overlapResultsCount > 0;
    }

    private int OverlapCircle(Vector3 position)
    {
        return Physics2D.OverlapCircleNonAlloc(position, _circleRadius, _overlapResults, _searchLayerMask.value);
    }

    private void TryAttackEnemies()
    {
        for (var i = 0; i < _overlapResultsCount; i++)
        {
            if (_overlapResults[i].TryGetComponent(out IDamageable damageable) == false)
                continue;

            if (_considerObstacles)
            {
                var startPointPosition = _overlapStartPoint.position;
                var colliderPosition = _overlapResults[i].transform.position;
                var hasObstacle = Physics2D.Linecast(startPointPosition, colliderPosition, _obstacleLayerMask.value);

                if (hasObstacle)
                    continue;
                
            }

            damageable.ApplyDamage(_damage);
        }
    }

    private void OnDrawGizmos()
    {
        TryDrawGizmos(DrawGizmosType.Always);
    }

    private void OnDrawGizmosSelected()
    {
        TryDrawGizmos(DrawGizmosType.OnSelected);
    }

    private void TryDrawGizmos(DrawGizmosType requiredType)
    {
        if (drawGizmosType != requiredType)
            return;

        if (_overlapStartPoint == null)
            return;

        Gizmos.matrix = _overlapStartPoint.localToWorldMatrix;
        Gizmos.color = _gizmoColor;

        Gizmos.DrawWireSphere(_offset, _circleRadius);
    }
}
