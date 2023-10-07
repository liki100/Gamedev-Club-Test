using UnityEngine;

[CreateAssetMenu(menuName = "Data/Weapon Info", fileName = "_weaponInfo")]
public class WeaponInfo : DefaultItemInfo
{
    [SerializeField] private Vector3 _muzzle;
    [SerializeField, Min(0f)] private float _damage;
    [SerializeField, Min(0f)] private float _fireRate;
    [SerializeField, Min(0f)] private float _reloadTime;
    [SerializeField, Min(1)] private int _ammo;
    
    public Vector3 Muzzle => _muzzle;
    public float Damage => _damage;
    public float FireRate => _fireRate;
    public float ReloadTime => _reloadTime;
    public int Ammo => _ammo;
}