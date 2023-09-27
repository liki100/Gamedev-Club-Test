using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMoverAndRotater : MonoBehaviour
{
    [Header("Common")] 
    [SerializeField] private bool _facingRight;
    [SerializeField] private Transform _weapon;
    [SerializeField] private GameObject _skin;
    [SerializeField] private float _speed = 150f;

    [Header("Masks")] 
    [SerializeField] private LayerMask _searchLayerMask;
    
    [Header("Overlap Area")] 
    [SerializeField] private Transform _overlapStartPoint;
    [SerializeField, Min(0)] private float _circleRadius = 1f;

    private readonly Collider2D[] _overlapResults = new Collider2D[32];
    private int _overlapResultsCount;
    
    private Rigidbody2D _rigidbody;
    private Joystick _joystick;
    private Vector3 _moveDirection;

    public void Init()
    {
        _joystick = ServiceLocator.Current.Get<Joystick>();
    }
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        _moveDirection = new Vector3(_joystick.Horizontal, _joystick.Vertical, 0f);
        
        if (TryFindEnemies())
        {
            var targets = new Dictionary<Vector2, float>();

            for (var i = 0; i < _overlapResultsCount; i++)
            {
                if (_overlapResults[i].TryGetComponent(out IDamageable damageable) == false)
                    continue;
                
                var distance = Vector3.Distance(transform.position, _overlapResults[i].gameObject.transform.position);
                
                var direction = _overlapResults[i].gameObject.transform.position - _weapon.transform.position;

                targets.TryAdd(direction, distance);
            }

            var minDirection = targets.OrderBy(k => k.Value).First();
            
            RotateCharacterAndWeapon(minDirection.Key);
        }
        else
        {
            if (_moveDirection == Vector3.zero)
                return;
            
            RotateCharacterAndWeapon(_moveDirection);
        }
    }
    
    private void FixedUpdate()
    {
        _rigidbody.velocity = _moveDirection.normalized * (_speed * Time.fixedDeltaTime);
    }
    
    private void Flip()
    {
        _facingRight = !_facingRight;
        
        _skin.transform.Rotate(0f,180f, 0f);
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

    private void RotateCharacterAndWeapon(Vector3 direction)
    {
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _weapon.rotation = Quaternion.Euler(0f, 0f, angle);

        var localScale = Vector3.one;
            
        if (angle > 90 || angle < -90)
        {
            localScale.y = -1;
        }
        else
        {
            localScale.y = 1;
        }

        _weapon.transform.localScale = localScale;
        
        switch (direction.x)
        {
            case > 0 when !_facingRight:
            case < 0 when _facingRight:
                Flip();
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(_overlapStartPoint.transform.position, _circleRadius);
    }
}
