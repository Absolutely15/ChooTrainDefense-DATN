public interface IChest
{
    ChestType ChestType { get; set; }
    void Init();
    void Open();
    void Claim();
}

public enum ChestType
{
    DroneChest,
    CurrencyChest
}
