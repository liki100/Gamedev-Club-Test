using UnityEngine;

[RequireComponent(typeof(CharacterInputController), typeof(Rigidbody2D))]
public class CharacterMover : MonoBehaviour
{
    [SerializeField] private float _speed = 150f;
    
    private CharacterInputController _controller;
    private Rigidbody2D _rigidbody;
    
    private void Start()
    {
        _controller = GetComponent<CharacterInputController>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    
    private void FixedUpdate()
    {
        _rigidbody.velocity = _controller.GetDirection().normalized * (_speed * Time.fixedDeltaTime);
    }
}