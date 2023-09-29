using UnityEngine;
using UnityEngine.UI;

public class MonstersHealthBar : MonoBehaviour
{
    [SerializeField] private Monster _monster;
    [SerializeField] private Image _healthBar;

    private void Awake()
    {
        _monster.OnHealthChangedEvent += OnHealthChanged;
    }

    private void OnHealthChanged(float health)
    {
        _healthBar.fillAmount = health;
    }
}
