using System.Collections.Generic;
using DG.Tweening;
using UnityBase.DesignPattern;
using UnityEngine;
using static NVTT.Utilities;


[DefaultExecutionOrder(-5)]
public class GPRainbowDefense : GPBase
{
    #region Properties
    [Header("Ref")]
    public VariableJoystick variableJoystick;
    public PlayerController player;
    public DroneController drone;
    public GameTutorial tutorial;
    public MapManager map;

    public static readonly List<int> CheckPoint = new List<int> {0, 10, 30};
    
    [HideInInspector] public int levelID;
    [HideInInspector] public List<Projectile> activeProj = new List<Projectile>();

    #endregion

    #region Init
    private void Awake()
    {
        PlayerSave.InitDefault();
        GetLevelID();

        if (!IsPassBasicTutorial)
            Instantiate(tutorial.gameObject);

        if (CheckPoint.Contains(PlayerSave.CurrentGameLevel) && !PlayerSave.AlreadyInitMapData)
        {
            PlayerSave.SerializeMapSaveData(GameDB.mapData.defaultMapSaveData[levelID]);
            PlayerSave.AlreadyInitMapData = true;
        }
        
        map = Instantiate(GameDB.mapData.map[levelID]);
        DroneUnlockedCheck();
    }

    public override void OnInitializationGame()
    {
        base.OnInitializationGame();
        InitObserver();
        InitWeaponProj();
    }

    private void InitObserver()
    {
        Observer.Instance.AddObserver(EventID.EndGameLevel, OnEndGame);
        Observer.Instance.AddObserver(EventID.ChangeWeapon, ChangeWeapon);
    }
    
    private void GetLevelID()
    {
        if (PlayerSave.CurrentGameLevel < 10)
            levelID = 0;
        if (PlayerSave.CurrentGameLevel >= 10 && PlayerSave.CurrentGameLevel < 30)
            levelID = 1;
        else if (PlayerSave.CurrentGameLevel >= 30)
            levelID = 2;
    }
    
    #region Weapon Init
    private void AddWeaponProjToList(Weapon weapon) => weapon.onFireProjectile.AddListener(w => activeProj.Add(w.projectile));
    private static void RemoveWeaponProjEvent(Weapon weapon) => weapon.onFireProjectile.RemoveAllListeners();

    private void InitWeaponProj()
    {
        foreach (var t in map.guard)
        {
            AddWeaponProjToList(t.guardWeapon.weapon);
        }
        AddWeaponProjToList(player.CurrentWeapon);
    }

    private void ChangeWeapon()
    {
        foreach (var weapon in player.weaponController.weaponList)
        {
            RemoveWeaponProjEvent(weapon);
        }
        AddWeaponProjToList(player.CurrentWeapon);
    }
    #endregion

    #region Drone Init
    public void DroneUnlockedCheck()
    {
        if (!PlayerSave.IsDroneUnlocked) return;
        drone.gameObject.SetActive(true);
        AddWeaponProjToList(drone.droneWeapon.weapon);
    }
    #endregion
    
    #endregion

    #region Start / End Game Level

    public override void OnStartGame()
    {
        base.OnStartGame();
        
        // if (PlayerSave.CurrentGameLevel >= 1)
        //     Advertisements.Instance.ShowInterstitial(map.zombiePool.StartSpawn, "Start_Level");
        // else
        map.zombieSpawner.StartSpawn();
        Observer.Instance.Notify(EventID.StartGameLevel);
    }

    public override void OnEndGame()
    {
        base.OnEndGame();
        PlayerSave.CurrentGameLevel++;
        
        var slowMotion = DOTween.Sequence();
        slowMotion.AppendCallback(() => Time.timeScale = 0.2f)
            .AppendInterval(2)
            .AppendCallback(() => Time.timeScale = 1f).SetUpdate(true);
    }

    #endregion
    
    public override void CheckGameLogic()
    {
        base.CheckGameLogic();
        
        ProjectileMove();
    }

    private void ProjectileMove()
    {
        if (activeProj.Count == 0) return;
        for (var i = 0; i < activeProj.Count; i++)
        {
            var curProj = activeProj[i];
            if (!(curProj.DoMove() >= 1) && !curProj.alreadyDealtDamage) continue;
            curProj.Dispose();
            activeProj.RemoveAt(i);
        }
    }
}
