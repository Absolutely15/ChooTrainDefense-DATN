using NVTT;
using Sirenix.OdinInspector;
using TMPro;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]
public class EnvironmentProgress : MonoBehaviour
{
    [Title("Environment")]
    public GameObject environmentPanel;
    public TextMeshProUGUI level;
    public Image currEnvironmentImg;
    public Image nextEnvironmentImg;
    public Image[] environmentBarSteps;
    public Sprite[] environmentBarStepFillSprites;
    public Sprite[] environmentImg;


    private void Start()
    {
        UpdateBar();
        SetCurrentEnvironmentImage(environmentImg[Utilities.MapManager.id]);
        SetNextEnvironmentImage(environmentImg[Utilities.MapManager.id + 1]);
        Observer.Instance.AddObserver(EventID.EndGameLevel, UpdateBar);
    }


    public void SetCurrentEnvironmentImage(Sprite sprite)
    {
        currEnvironmentImg.sprite = sprite;
    }

    public void SetNextEnvironmentImage(Sprite sprite)
    {
        nextEnvironmentImg.sprite = sprite;
    }
    public void FillEnvironmentBar(int n)
    {
        n %= environmentBarSteps.Length;
        for (var i = 0; i < environmentBarSteps.Length; i++)
            environmentBarSteps[i].sprite = environmentBarStepFillSprites[i < n ? 0 : i == n ? 1 : 2];
    }

    private void UpdateBar()
    {
        FillEnvironmentBar(PlayerSave.CurrentGameLevel);
        level.text = $"LEVEL {PlayerSave.CurrentGameLevel + 1}";
    }
}
