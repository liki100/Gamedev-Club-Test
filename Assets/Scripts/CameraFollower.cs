using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothing = 1f;
    
    private void FixedUpdate()
    {
        if (_targetTransform == null)
            return;
        
        Move();
    }

    private void Move()
    {
        var nextPosition = Vector3.Lerp(transform.position, _targetTransform.position + _offset, Time.fixedDeltaTime * _smoothing);

        transform.position = nextPosition;
    }
}
