public interface IUpgradeStat
{
    EventID EventID { get; set; }
    string Description { get; set; }
    int LevelMax { get; set; }
    int CurLevelUI { get; set; }
    int GoldCost { get; set; }
    int UpgradeLevel { get; set; }
    bool IsInit { get; set; }
    void Init();
    void LoadData();
    void UpgradeSuccess();
}
