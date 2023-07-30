using NVTT;
using UnityBase.DesignPattern;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [HideInInspector] public Health zombie;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private Weapon weapon;
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private ParticleSystem[] effects;
    
    // public float mainTextureLength = 1f;
    // public float noiseTextureLength = 1f;
    // private Vector4 length = new Vector4(1,1,1,1);
    private bool saver = true;
    private bool playerIsDead;

    // private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    // private static readonly int Noise = Shader.PropertyToID("_Noise");

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (playerIsDead) return;
        SpawnVFX();

        if (weapon.WeaponTarget != null || saver) return;
        StopVFX();
    }

    private void Init()
    {
        Utilities.Player.health.onDeath.AddListener(OnPlayerDead);
        Utilities.Player.health.onRevive.AddListener(OnPlayerRevive);
        GPExecutor.Instance.onPauseGame.AddListener(StopAudio);
        GPExecutor.Instance.onResumeGame.AddListener(OnResumeGame);
        Observer.Instance.AddObserver(EventID.TryWeapon, StopAudio);
    }
    
    private void SpawnVFX()
    {
        if (weapon.WeaponTarget == null) return;
        // laserLine.material.SetTextureScale(MainTex, new Vector2(length[0], length[1]));                    
        // laserLine.material.SetTextureScale(Noise, new Vector2(length[2], length[3]));
        //To set LineRender position
        laserLine.SetPosition(0, weapon.projectileSpawnPoint.position);
        //End laser position if collides with object
        var weaponTargetPos = weapon.WeaponTarget.position + Vector3.up * 0.25f;
        laserLine.SetPosition(1, weaponTargetPos);
        hitEffect.transform.SetPositionAndRotation(weaponTargetPos, Quaternion.identity);
        
        //Texture tiling
        // var pos = Utilities.Player.transform.position;
        // length[0] = mainTextureLength * Vector3.Distance(pos, weaponTargetPos);
        // length[2] = noiseTextureLength * Vector3.Distance(pos, weaponTargetPos);
        if (laserLine.enabled) return;
        foreach (var allPs in effects)
        {
            if (!allPs.isPlaying) allPs.Play();
        }
        PlayAudio();
        saver = false;
        laserLine.enabled = true;
    }

    private void StopVFX()
    {
        laserLine.enabled = false;
    
        foreach (var allPs in effects)
        {
            if (allPs.isPlaying) allPs.Stop();
        }
        
        AudioManager.Instance.StopAudio(AudioType.LaserGun);
        saver = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        var enemies = other.GetComponent<Health>();
        if (enemies == null) return;
        if (enemies.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            zombie = enemies;
    }

    private void OnTriggerExit(Collider other)
    {
        zombie = null;
    }

    private void OnPlayerDead(Health h)
    {
        StopVFX();
        playerIsDead = true;
    }

    private void OnPlayerRevive(Health h)
    {
        playerIsDead = false;
    }

    #region Audio

    private static void PlayAudio() => AudioManager.Instance.PlayAudio(AudioType.LaserGun);
    private static void StopAudio() => AudioManager.Instance.StopAudio(AudioType.LaserGun);
    private void OnResumeGame()
    {
        if (PlayerSave.GetWeaponIsUsing() != weapon.weaponLocalData.id) return;
        if (!laserLine.enabled) return;
        AudioManager.Instance.PlayAudio(AudioType.LaserGun);
    }
    #endregion
}
