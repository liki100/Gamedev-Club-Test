using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : MonoBehaviour, IService
{
    [SerializeField] private WeaponInventoryItemInfo _data;
    [SerializeField] private SpriteRenderer _weaponSkin;
    [SerializeField] private Projectile _projectileTemplate;
    [SerializeField] private ForceMode2D _forceMode = ForceMode2D.Impulse;
    [SerializeField, Min(0f)] private float _force = 10f;

    private float _currentFireRateTime;
    private int _currentAmmo;
    private bool _isShooting;
    private bool _isReloading;
    private Transform _muzzle;

    private EventBus _eventBus;

    public void Init()
    {
        _currentAmmo = _data.Ammo;
        _currentFireRateTime = _data.FireRate;
        _weaponSkin.sprite = _data.SpriteIcon;
        
        if (_muzzle == null)
        {
            _muzzle = new GameObject("Muzzle").transform;
            _muzzle.parent = _weaponSkin.transform;
        }
        
        _muzzle.localPosition = _data.Muzzle;
        _eventBus = ServiceLocator.Current.Get<EventBus>();
        _eventBus.Invoke(new AmmoChangedSignal(_currentAmmo, _data.Ammo));

        var slot = ServiceLocator.Current.Get<Character>().Inventory.GetWeaponSlot();

        var item = new InventoryItem(_data);
        item.State.Amount = 1;
        item.State.isEquipped = true;
        slot.SetItem(item);
    }

    private void Update()
    {
        _currentFireRateTime -= Time.deltaTime;
        
        if (!_isShooting) 
            return;

        if (_currentFireRateTime > 0)
            return;
        
        if (_isReloading)
            return;

        if (_currentAmmo <= 0)
        {
            SetReload();
            return;
        }
        
        PerformAttack();
        
        _currentAmmo--;
        _eventBus.Invoke(new AmmoChangedSignal(_currentAmmo, _data.Ammo));
        _currentFireRateTime = _data.FireRate;
    }
    
    private void PerformAttack()
    {
        var projectile = Instantiate(_projectileTemplate, _muzzle.position, _muzzle.rotation);

        projectile.Init(_data.Damage);
        
        projectile.Rigidbody.AddForce(_muzzle.right * _force, _forceMode);
    }
    
    private IEnumerator Reload()
    {
        if (_currentAmmo == _data.Ammo) 
            yield break;
        
        _isReloading = true;
        yield return new WaitForSeconds(_data.ReloadTime); 
        _currentAmmo = _data.Ammo;
        _eventBus.Invoke(new AmmoChangedSignal(_currentAmmo, _data.Ammo));
        _isReloading = false;
    }

    public void SetReload()
    {
        StartCoroutine(Reload());
    }

    public void SetShoot(bool isShooting)
    {
        _isShooting = isShooting;
    }

    public SaveManager.WeaponData GetData()
    {
        var data = new SaveManager.WeaponData()
        {
            Ammo = _currentAmmo,
            FireRateTime = _currentFireRateTime
        };
        
        return data;
    }

    public void SetInfo(WeaponInventoryItemInfo info)
    {
        _data = info;
        Init();
    }

    public void SetData(SaveManager.WeaponData data)
    {
        _currentAmmo = data.Ammo;
        _currentFireRateTime = data.FireRateTime;
        _eventBus.Invoke(new AmmoChangedSignal(_currentAmmo, _data.Ammo));
    }
}
