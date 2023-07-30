public enum AudioType
{
    None,
    Background,
    PlayerDeath,
    PlayerRevive,

    #region Click
    Click,
    ClickUpgrade,
    Equip,
    #endregion

    #region Weapon
    Rifle,
    Shotgun,
    Uzi,
    Rocket,
    QuadRocket,
    Flamethrower,
    LaserGun,
    #endregion

    #region Drone Weapon
    DroneRifle,
    #endregion

    #region Map Objects
    CollectSingleCoin,
    CollectAllCoin,
    RebuildBarrier,
    RebuildAllBarrier,
    #endregion
    
    #region Chest
    ChestFall,
    ChestOpening,
    ChestReward
    #endregion

}
