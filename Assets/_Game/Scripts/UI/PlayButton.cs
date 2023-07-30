using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NVTT;
using TMPro;
using UnityBase.DesignPattern;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private Sprite[] iconSprites;
    [SerializeField] private Sprite[] bgSprites;
    [SerializeField] private GameObject[] particle;
    [SerializeField] private Button button;
    [SerializeField] private Image bg;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI desc;
    
    private static readonly List<int> LevelBoss = new List<int> { 10, 20, 30, 40, 50 };
    private static readonly List<int> NextMapCp = new List<int> { 11, 31 };

    private void OnValidate()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(GPExecutor.Instance.StartGame);
    }

    private void OnEnable()
    {
        if (LevelBoss.Contains(PlayerSave.CurrentGameLevel + 1))
        {
            SetButtonTheme(1, "Start");
        }
        else if (NextMapCp.Contains(PlayerSave.CurrentGameLevel + 1) && Utilities.MapManager.id == NextMapCp.IndexOf(PlayerSave.CurrentGameLevel + 1))
        {
            SetButtonTheme(2, "Next World");
            NextWorldFunction();
        }
        else
        {
            SetButtonTheme(0, "Start");
        }
    }

    private void SetButtonTheme(int j, string description)
    {
        for (var i = 0; i < particle.Length; i++)
        {
            particle[i].gameObject.SetActive(i == j);
        }

        bg.sprite = bgSprites[j];
        icon.sprite = iconSprites[j];
        desc.text = description;
    }

    private void NextWorldFunction()
    {
        desc.fontSize = 45;
        button.onClick.RemoveListener(GPExecutor.Instance.StartGame);
        button.onClick.AddListener(NextWorldBtnOnClick);
    }

    private void NextWorldBtnOnClick()
    {
        PlayerSave.AlreadyInitMapData = false;
        gameObject.SetActive(false);
        GPExecutor.Instance.Replay();
    }
}
