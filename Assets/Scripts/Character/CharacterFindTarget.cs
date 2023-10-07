using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterInputController), typeof(CharacterRotate))]
public class CharacterFindTarget : MonoBehaviour
{
    [SerializeField] private LayerMask _searchLayerMask;
    [SerializeField] private Transform _overlapStartPoint;
    [SerializeField, Min(0)] private float _circleRadius = 1f;

    private readonly Collider2D[] _overlapResults = new Collider2D[32];
    private int _overlapResultsCount;
    
    private CharacterInputController _controller;
    private CharacterRotate _rotate;
    
    private void Start()
    {
        _controller = GetComponent<CharacterInputController>();
        _rotate = GetComponent<CharacterRotate>();
    }
    
    private void Update()
    {
        if (TryFindEnemies())
        {
            var targets = new Dictionary<Vector2, float>();

            for (var i = 0; i < _overlapResultsCount; i++)
            {
                if (_overlapResults[i].TryGetComponent(out IDamageable damageable) == false)
                    continue;
                
                var distance = Vector3.Distance(transform.position, _overlapResults[i].gameObject.transform.position);
                
                var direction = _overlapResults[i].gameObject.transform.position - _overlapStartPoint.position;

                targets.TryAdd(direction, distance);
            }

            var minDirection = targets.OrderBy(k => k.Value).First();
            
            _rotate.ToTarget(minDirection.Key);
        }
        else
        {
            if (_controller.GetDirection() == Vector3.zero)
                return;
            
            _rotate.ToTarget(_controller.GetDirection());
        }
    }

    private bool TryFindEnemies()
    {
        var position = _overlapStartPoint.position;

        _overlapResultsCount = OverlapCircle(position);

        return _overlapResultsCount > 0;
    }
    
    private int OverlapCircle(Vector3 position)
    {
        return Physics2D.OverlapCircleNonAlloc(position, _circleRadius, _overlapResults, _searchLayerMask.value);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(_overlapStartPoint.transform.position, _circleRadius);
    }
}