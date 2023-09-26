using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;
    
    private EventBus _eventBus;
    
    public void Init()
    {
        _eventBus = ServiceLocator.Current.Get<EventBus>();
        _eventBus.Subscribe<AmmoChangedSignal>(DisplayAmmo);
    }

    private void DisplayAmmo(AmmoChangedSignal signal)
    {
        _label.text = $"{signal.Value}/{signal.MaxValue}";
    }
    
    private void OnDestroy()
    {
        _eventBus.Unsubscribe<AmmoChangedSignal>(DisplayAmmo);
    }
}
