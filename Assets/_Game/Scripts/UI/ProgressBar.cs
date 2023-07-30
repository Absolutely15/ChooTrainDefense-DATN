using TMPro;
using UnityBase.DesignPattern;
using NVTT;

public class ProgressBar : UIFillAnimation
{
    public TextMeshProUGUI curLevel;
    public TextMeshProUGUI nextLevel;

    private int zombieKilled;
    private bool isStartLevel = true;
    private void Start()
    {
        Observer.Instance.AddObserver(EventID.StartGameLevel, ResetCounter);
        Observer.Instance.AddObserver(EventID.ZombieDead, UpdateProgressBar);
        Observer.Instance.AddObserver(EventID.Dead, OnPlayerDeath);
    }

    private void OnEnable()
    {
        //progress bar
        if (isStartLevel)
            SetTargetValue(1, true);
        
        SetTargetValue(0);

        curLevel.text = (PlayerSave.CurrentGameLevel + 1).ToString();
        nextLevel.text = (PlayerSave.CurrentGameLevel + 2).ToString();
    }

    private void ResetCounter()
    {
        isStartLevel = true;
        zombieKilled = 0;
    }

    private void UpdateProgressBar()
    {
        zombieKilled++;
        SetTargetValue((float) zombieKilled / Utilities.MapManager.zombieSpawner.totalZombieInWave);
    }

    private void OnPlayerDeath() => isStartLevel = false;
    
}
