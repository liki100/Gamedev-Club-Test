using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBar;

    private EventBus _eventBus;

    public void Init()
    {
        _eventBus = ServiceLocator.Current.Get<EventBus>();
        _eventBus.Subscribe<HealthChangedSignal>(DisplayHealth);
    }

    private void DisplayHealth(HealthChangedSignal signal)
    {
        _healthBar.fillAmount = signal.Value;
    }

    private void OnDestroy()
    {
        _eventBus.Unsubscribe<HealthChangedSignal>(DisplayHealth);
    }
}