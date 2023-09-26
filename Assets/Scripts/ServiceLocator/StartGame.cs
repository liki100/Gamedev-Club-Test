using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class StartGame : MonoBehaviour
{
    [SerializeField] private Character _character;
    [SerializeField] private RangeWeapon _rangeWeapon;
    [SerializeField] private Joystick _joystick;

    [SerializeField] private CharacterMoverAndRotater _mover;
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private AmmoUI _ammoUI;
    [SerializeField] private UIInventory _uiInventory;
    [SerializeField] private Spawner _spawner;
    [SerializeField] private SaveManager _save;
    
    private EventBus _eventBus;

    private void Awake()
    {
        _eventBus = new EventBus();
        
        RegisterServices();
        Init();
        _save.Load();
        
        _eventBus.Subscribe<PlayerDeadSignal>(OnRestartGame);
    }

    private void RegisterServices()
    {
        ServiceLocator.Initialize();

        ServiceLocator.Current.Register(_eventBus);
        ServiceLocator.Current.Register(_character);
        ServiceLocator.Current.Register(_rangeWeapon);
        ServiceLocator.Current.Register(_joystick);
        ServiceLocator.Current.Register(_spawner);
        ServiceLocator.Current.Register(_uiInventory);
    }
    
    private void Init()
    {
        _healthBar.Init();
        _ammoUI.Init();
        _character.Init();
        _rangeWeapon.Init();
        _mover.Init();
        _uiInventory.Init();
        _spawner.Init();
    }
    
    private void OnDisable()
    {
        _eventBus.Unsubscribe<PlayerDeadSignal>(OnRestartGame);
    }

    private void OnRestartGame(PlayerDeadSignal signal)
    {
        _save.Delete();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
