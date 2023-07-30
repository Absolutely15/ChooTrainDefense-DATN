public enum EventID
{
    #region Basic
    None,
    StartGameLevel,
    EndGameLevel,
    EndGameLevelData,
    #endregion

    #region Player
    Revive,
    Dead,
    ChangeWeapon,
    ChangeSkin,
    RestoreFullHealth,
    #endregion

    #region Map
    ZombieDead,
    UpgradeWeaponGlobal,
    UpgradeStat,
    UpgradeDefense,
    UpgradeBarrier,
    UpgradeTrap,
    RebuildBarrier,
    RebuildAllBarrier,
    InUpgradeZone,
    OutUpgradeZone,
    EnterTeleport,
    ExitTeleport,
    #endregion
    
    #region UI
    PopUpBonusGold,
    DismissBonusCurrency,
    TryWeapon,
    GoldChange,
    DiamondChange,
    #endregion

    #region Extra (Currently Not Using)
    RestoreSingleHealth,
    BonusSpeed,
    DismissBonusSpeed,
    #endregion

}