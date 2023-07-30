using System.Collections.Generic;
using NVTT;
using Sirenix.OdinInspector;
using UnityBase.DesignPattern;
using UnityEngine;

public class Flamethrower : Weapon
{
    #region Properties
    [Title("Flamethrower")]
    [HideInInspector] public List<Health> enemyList;
    [SerializeField] private ParticleSystem flame;
    private bool saver = true;
    #endregion
    
    #region Unity Functions
    private void Start()
    {
        Init();
    }
    
    public override void Update()
    {
        for (var i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].currentHealth <= 0)
            {
                enemyList.RemoveAt(i);
            }
        }

        SpawnVFX();
        DealDamage((int)(globalDamage * localDamage));

        if (WeaponTarget != null || saver) return;
        StopFX();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var enemies = other.GetComponent<Health>();
        if (enemies == null) return;
        if (enemies.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            enemyList.Add(enemies);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var enemies = other.GetComponent<Health>();
        if (enemies == null) return;
        if (enemies.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            enemyList.Remove(enemies);
        }
    }
    #endregion

    #region Flamethrower
    private void Init()
    {
        Utilities.Player.health.onDeath.AddListener(StopFX);
        GPExecutor.Instance.onPauseGame.AddListener(StopAudio);
        GPExecutor.Instance.onResumeGame.AddListener(OnResumeGame);
        Observer.Instance.AddObserver(EventID.TryWeapon, StopAudio);
    }
    
    private void DealDamage(int amount)
    {
        if (!(Time.time > LastFired + localFireRate * 1f / globalFireRate)) return;
        LastFired = Time.time;
        foreach (var t in enemyList)
        {
            t.SufferDamage(amount);
        }
    }
    
    private void SpawnVFX()
    {
        if (flame.isPlaying || enemyList.Count == 0) return;
        PlayFX();
    }
    
    private void PlayFX()
    {
        flame.Play();
        PlayAudio();
        saver = false;
    }

    private void StopFX(Health h = null)
    {
        flame.Stop();
        StopAudio();
        saver = true;
    }
    #endregion

    #region Audio
    private static void PlayAudio() => AudioManager.Instance.PlayAudio(AudioType.Flamethrower);
    private static void StopAudio() => AudioManager.Instance.StopAudio(AudioType.Flamethrower);
    private void OnResumeGame()
    {
        if (PlayerSave.GetWeaponIsUsing() != weaponLocalData.id) return;
        if (enemyList.Count == 0) return;
        AudioManager.Instance.PlayAudio(AudioType.Flamethrower);
    }
    #endregion

}