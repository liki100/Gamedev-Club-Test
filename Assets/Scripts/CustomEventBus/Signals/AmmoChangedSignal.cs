public class AmmoChangedSignal
{
    public readonly int Value;
    public readonly int MaxValue;
    
    public AmmoChangedSignal(int value, int maxValue)
    {
        Value = value;
        MaxValue = maxValue;
    }
}