using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Drop
{
    [SerializeField] private ItemInfo _info;
    [SerializeField] private int _amountMin;
    [SerializeField] private int _amountMax;

    public ItemInfo Info => _info;
    public int AmountMin => _amountMin;
    public int AmountMax => _amountMax;
}
