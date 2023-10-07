using UnityEngine;

public class CharacterInputController : MonoBehaviour
{
    private Joystick _joystick;
    private Vector3 _direction;
    private void Start()
    {
        _joystick = ServiceLocator.Current.Get<Joystick>();
    }

    private void Update()
    {
        _direction = new Vector3(_joystick.Horizontal, _joystick.Vertical, 0f);
    }

    public Vector3 GetDirection()
    {
        return _direction;
    }
}