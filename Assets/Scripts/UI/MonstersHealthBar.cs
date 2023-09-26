using UnityEngine;
using UnityEngine.UI;

public class MonstersHealthBar : MonoBehaviour
{
    [SerializeField] private Monster _monster;
    [SerializeField] private Image _image;

    private void Awake()
    {
        _monster.OnHealthChangedEvent += OnHealthChanged;
    }

    private void OnHealthChanged(float health)
    {
        _image.fillAmount = health;
    }
}
