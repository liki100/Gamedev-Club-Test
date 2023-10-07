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
    
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private AmmoUI _ammoUI;
    [SerializeField] private UIInventory _uiInventory;
    [SerializeField] private Spawner _spawnerMonsters;
    [SerializeField] private SpawnerItems _spawnerItems;
    [SerializeField] private SaveManager _save;
    
    private EventBus _eventBus;

    private void Awake()
    {
        _eventBus = new EventBus();

        RegisterServices();
        Init();
        _save.Load();
        
        _eventBus.Subscribe<CharacterDeadSignal>(OnRestartGame);
        _eventBus.Subscribe<AllMonstersDeadSignal>(OnNextGame);
    }

    private void RegisterServices()
    {
        ServiceLocator.Initialize();

        ServiceLocator.Current.Register(_eventBus);
        ServiceLocator.Current.Register(_character);
        ServiceLocator.Current.Register(_rangeWeapon);
        ServiceLocator.Current.Register(_joystick);
        ServiceLocator.Current.Register(_spawnerMonsters);
        ServiceLocator.Current.Register(_spawnerItems);
        ServiceLocator.Current.Register(_uiInventory);
    }
    
    private void Init()
    {
        _healthBar.Init();
        _ammoUI.Init();
        _character.Init();
        _rangeWeapon.Init();
        _uiInventory.Init();
        _spawnerMonsters.Init();
        _spawnerItems.Init();
    }
    
    private void OnDisable()
    {
        _eventBus.Unsubscribe<CharacterDeadSignal>(OnRestartGame);
        _eventBus.Unsubscribe<AllMonstersDeadSignal>(OnNextGame);
    }

    private void OnRestartGame(CharacterDeadSignal signal)
    {
        _save.Delete();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void OnNextGame(AllMonstersDeadSignal signal)
    {
        _spawnerMonsters.SpawnNumberMonsters();
    }
}
