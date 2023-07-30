using DG.Tweening;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Projectile : MonoBehaviour
{
    #region Properties
    public bool impactVFXPlay = true;
    public bool fixedFlyTime;
    public bool dealDamageMultipleTimes;
    public float flyTime = 1f;
    public float disposingTime = 0.5f;
    public GameObject model;
    public ParticleSystem trailVFX;
    public ParticleSystem impactVFX;
    public AudioSource hitVFX;
    
    [HideInInspector] public Weapon weapon;
    [HideInInspector] public float speed;
    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Vector3 destination;
    [HideInInspector] public bool alreadyDealtDamage;
    
    [SerializeField] private ITrajectory trajectory;
    private IDealDamage dealDamage;
    
    private float Dist => (startPos - destination).magnitude;
    private float TotalDamage => weapon.globalDamage * weapon.localDamage;
    private float progress;
    private bool disposing;
    #endregion
    
    private void OnValidate()
    {
        if (trajectory == null)
        {
            trajectory = GetComponent<ITrajectory>();
        }
    }

    private void Awake()
    {
        weapon = GetComponentInParent<Weapon>();
        dealDamage = GetComponent<IDealDamage>();
    }

    private void OnEnable()
    {
        if (trailVFX != null)
            trailVFX.Play();
        
        if (model != null)
            model.SetActive(true);
        
        transform.position = weapon.projectileSpawnPoint.position;
        alreadyDealtDamage = false;
        progress = 0;

    }

    private void Start()
    {
        transform.SetParent(null);
    }

    public void Init(WeaponLocalData weaponLocalData)
    {
        speed = weaponLocalData.projectileMoveSpeed;
        trajectory.Init(this);
    }

    private void DoDamage(Health target, int amount)
    {
        dealDamage.DealDamage(target, amount);
    }

    public void Dispose()
    {
        disposing = true;
        
        if (hitVFX != null)
            hitVFX.Play();
        
        if (trailVFX != null)
            trailVFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        
        if (impactVFX != null)
        {
            if (impactVFXPlay)
                impactVFX.Play();
            else
                impactVFX.gameObject.SetActive(true);
        }
        
        if (model != null)
            model.SetActive(false);
        
        DOVirtual.DelayedCall(disposingTime, () =>
        {
            disposing = false;
            gameObject.SetActive(false);
            
            if (!impactVFXPlay && impactVFX != null)
                impactVFX.gameObject.SetActive(false);
        });
    }

    public float DoMove()
    {
        if (disposing)
            return 0;
        startPos = weapon.projectileSpawnPoint.position;
        
        progress = fixedFlyTime ? progress + Time.deltaTime / flyTime : Mathf.Min(progress + speed / Dist * Time.deltaTime, 1);
        trajectory.DoMove(startPos, destination, progress);

        return progress;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyDealtDamage) return;
        var enemies = other.GetComponent<Health>();
        if (enemies == null) return;

        DoDamage(enemies, (int) TotalDamage);
        
        if (dealDamageMultipleTimes) return;
        alreadyDealtDamage = true;
    }
}
