using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : MonoBehaviour, IService
{
    [SerializeField] private WeaponInfo _info;
    [SerializeField] private SpriteRenderer _weaponSkin;
    [SerializeField] private Projectile _projectileTemplate;
    [SerializeField] private ForceMode2D _forceMode = ForceMode2D.Impulse;
    [SerializeField, Min(0f)] private float _force = 10f;

    private float _currentFireRateTime;
    private int _currentAmmo;
    private bool _isShooting;
    private bool _isReloading;
    private Transform _muzzle;
    private Inventory _inventory;
    private Coroutine _reloadCoroutine;
    private EventBus _eventBus;

    public void Init()
    {
        _eventBus = ServiceLocator.Current.Get<EventBus>();
        _inventory = ServiceLocator.Current.Get<Character>().Inventory;
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
        _eventBus.Invoke(new AmmoChangedSignal(_currentAmmo, _info.Ammo));
        _currentFireRateTime = _info.FireRate;
    }
    
    private void PerformAttack()
    {
        var projectile = Instantiate(_projectileTemplate, _muzzle.position, _muzzle.rotation);

        projectile.Init(_info.Damage);
        
        projectile.Rigidbody.AddForce(_muzzle.right * _force, _forceMode);
    }
    
    public void SetReload()
    {
        _reloadCoroutine = StartCoroutine(Reload());
    }
    
    private IEnumerator Reload()
    {
        if (_currentAmmo == _info.Ammo) 
            yield break;
        
        _isReloading = true;
        yield return new WaitForSeconds(_info.ReloadTime); 
        _currentAmmo = _info.Ammo;
        _eventBus.Invoke(new AmmoChangedSignal(_currentAmmo, _info.Ammo));
        _isReloading = false;
    }

    public void SetShoot(bool isShooting)
    {
        _isShooting = isShooting;
    }

    public void UpdateData()
    {
        if (_reloadCoroutine != null)
        {
            StopCoroutine(_reloadCoroutine);
            _isReloading = false;
        }
        
        _info = (WeaponInfo)_inventory.GetSlotWithType(ItemType.Weapon).Item.Info;

        _currentAmmo = _info.Ammo;
        _currentFireRateTime = _info.FireRate;
        _weaponSkin.sprite = _info.SpriteIcon;
        
        if (_muzzle == null)
        {
            _muzzle = new GameObject("Muzzle").transform;
            _muzzle.parent = _weaponSkin.transform;
        }
        
        _muzzle.localPosition = _info.Muzzle;
        
        _eventBus.Invoke(new AmmoChangedSignal(_currentAmmo, _info.Ammo));
    }
    
    public SaveManager.WeaponData GetData()
    {
        var data = new SaveManager.WeaponData()
        {
            InfoId = _info.Id,
            Ammo = _currentAmmo,
            FireRateTime = _currentFireRateTime
        };
        
        return data;
    }

    public void SetData(SaveManager.WeaponData data)
    {
        _currentAmmo = data.Ammo;
        _currentFireRateTime = data.FireRateTime;
        UpdateData();
    }

    public void DefaultData()
    {
        var slot = _inventory.GetSlotWithType(ItemType.Weapon);
        var item = new InventoryItem(_info);
        slot.SetItem(item);
        UpdateData();
    }
}
