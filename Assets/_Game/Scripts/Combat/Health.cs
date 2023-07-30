using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    #region Properties

    [Title("Ref")]
    public GameObject model;

    [Title("Health")]
    public int currentHealth;
    public int maximumHealth;
    [Space]
    public float invincibilityDur;
    public bool invulnerable;
    public bool resetHealthOnEnable;
    [Title("Death")]
    public float delayBeforeDestruction;
    public bool revivable;
    public bool disableModelOnDeath;

    [Title("Event")]
    public UnityEvent onEnable = new UnityEvent();
    public UnityEvent onEnableModel = new UnityEvent();
    public UnityEvent<Health, int> onSufferDamage = new UnityEvent<Health, int>();
    public UnityEvent<Health> onDeath = new UnityEvent<Health>();
    public UnityEvent<Health> onRevive = new UnityEvent<Health>();

    private IHealthBar hpBar;
    
    #endregion

    #region Health Query
    public void SetHealth(int newValue) => currentHealth = newValue;
    public void SetMaxHealth(int newMaxValue) => maximumHealth = newMaxValue;
    public void SetUpgradeHealth(int newMaxValue) => currentHealth = maximumHealth = newMaxValue;
    public bool IsFullHealth() => currentHealth == maximumHealth;
    #endregion
    
    #region Damage Query

    private void DamageDisabled() => invulnerable = true;
    private void DamageEnabled() => invulnerable = false;

    private void DamageEnabled(float delay) => DOVirtual.DelayedCall(delay, () => invulnerable = false);
    #endregion
    
    #region Unity Functions
    private void Awake()
    {
        Init();
    }
    private void OnEnable()
    {
        if (resetHealthOnEnable)
        {
            SetHealth(maximumHealth);
        }
        DamageEnabled();
        onEnable.Invoke();
    }
    #endregion

    #region Core Feature
    private void Init()
    {
        SetHealth(maximumHealth);
        DamageEnabled();
        //UI
        var hpB =  GetComponentInChildren<IHealthBar>();
        if (hpB == null) return;
        hpBar = hpB;
        hpBar.Init();
    }

    private bool CanTakeDamageThisFrame()
    {
        if (invulnerable) return false;
        if (!enabled) return false;
        return currentHealth > 0;
    }

    public void SufferDamage(int amount, float invincibilityDuration = 0f)
    {
        if (!CanTakeDamageThisFrame())
        {
            return;
        }
        
        SetHealth(currentHealth - amount);
        
        //Visual
        hpBar?.ShowHP(currentHealth, maximumHealth);

        //Trigger Event
        onSufferDamage.Invoke(this, amount);
        
        //Player invincible ability
        if (invincibilityDuration > 0 && currentHealth >= 1)
        {
            DamageDisabled();
            DamageEnabled(invincibilityDuration);
        }
        
        //Dead
        if (currentHealth > 0) return;
        SetDeath();
    }
    
    public void Die()
    {
        onDeath.Invoke(this);
        
        DamageDisabled();
        if (revivable) return;

        if (delayBeforeDestruction > 0)
            DOVirtual.DelayedCall(delayBeforeDestruction, () => { DisableModelOnDeath(disableModelOnDeath); });
        else
            DisableModelOnDeath(disableModelOnDeath);

    }

    private void DisableModelOnDeath(bool disable)
    {
        if (disable)
        {
            model.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Revive(float invincibilityDuration = 2f)
    {
        DamageEnabled(invincibilityDuration);
        SetHealth(maximumHealth);
        hpBar?.ShowHP(currentHealth, maximumHealth);
        onRevive.Invoke(this);
    }

    #endregion

    #region Health Feature
    
    public void SetDeath()
    {
        SetHealth(0);
        Die();
    }
    
    public void ResetHealthWithModel()
    {
        SetHealth(maximumHealth);
        model.SetActive(true);
        DamageEnabled();
        hpBar?.ShowHP(currentHealth, maximumHealth);
        onEnableModel.Invoke();
    }

    public void RestoreHealth(int target)
    {
        SetHealth(target);
        hpBar?.ShowHP(currentHealth, maximumHealth);
    }
    #endregion
}
