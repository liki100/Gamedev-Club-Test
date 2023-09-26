using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private List<GameObject> panels;
    [SerializeField] private Button _buttonOpen;
    [SerializeField] private Button _buttonClose;

    private void Start()
    {
        _buttonOpen.onClick.AddListener(() => Show(true));
        _buttonClose.onClick.AddListener(() => Show(false));
    }

    private void OnDisable()
    {
        _buttonOpen.onClick.RemoveListener(() => Show(true));
        _buttonClose.onClick.RemoveListener(() => Show(false));
    }

    private void Show(bool active)
    {
        Time.timeScale = active ? 0 : 1;
        
        _panel.SetActive(active);
        foreach (var panel in panels)
        {
            panel.SetActive(!active);
        }
    }
}
