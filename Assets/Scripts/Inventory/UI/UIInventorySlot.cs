using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour
{
    [SerializeField] private Image _imageIcon;
    [SerializeField] private TMP_Text _textAmount;
    [SerializeField] private Button _button;

    private IInventorySlot _slot;
    public Button Button => _button;

    public void SetSlot(IInventorySlot slot)
    {
        _slot = slot;
    }
    
    public void Refresh()
    {
        if (_slot.IsEmpty)
        {
            Cleanup();
            return;
        }
        _imageIcon.sprite = _slot.Item.Info.SpriteIcon;
        _textAmount.text = $"x{_slot.Amount}";

        var textAmountEnabled = _slot.Amount > 1;
        _textAmount.gameObject.SetActive(textAmountEnabled);
        _imageIcon.gameObject.SetActive(true);
        _button.interactable = true;
    }

    private void Cleanup()
    {
        _imageIcon.gameObject.SetActive(false);
        _textAmount.gameObject.SetActive(false);
        _button.interactable = false;
    }
}