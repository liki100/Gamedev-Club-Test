using UnityEngine;

public class CharacterRotate : MonoBehaviour
{
    [SerializeField] private bool _facingRight;
    [SerializeField] private Transform _weapon;
    [SerializeField] private GameObject _skin;
    
    public void ToTarget(Vector3 direction)
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
    
    private void Flip()
    {
        _facingRight = !_facingRight;
        
        _skin.transform.Rotate(0f,180f, 0f);
    }
}