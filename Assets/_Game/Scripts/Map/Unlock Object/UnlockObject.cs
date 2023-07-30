using ATSoft;
using TMPro;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.UI;
using static NVTT.UIUtilities;

public class UnlockObject : MonoBehaviour
{
    [SerializeField] private DotweenScale canvasPopUp;
    [SerializeField] private RectTransform unlockBtnTransform;
    [SerializeField] private Button unlockBtn;
    [SerializeField] private TextMeshProUGUI unlockCostText;
    [SerializeField] private TextMeshProUGUI unlockCostGroundText;

    private IUnlockObject iUnlockObject;
    private bool inUnlockZone;
    private void Start()
    {
        Init();
        InitEvent();
    }

    private void Init()
    {
        iUnlockObject = GetComponent<IUnlockObject>();
        iUnlockObject.Init();
        
        //UI
        canvasPopUp.gameObject.SetActive(false);
        unlockCostGroundText.text = iUnlockObject.UnlockCost.ToString();
    }

    private void InitEvent()
    {
        unlockBtn.onClick.AddListener(OnClick);
        Observer.Instance.AddObserver(EventID.GoldChange, OnGoldChange);
    }

    private void OnClick()
    {
        if (!unlockBtnTransform.InteractableBtnResponse(iUnlockObject.UnlockCost)) return;
        PlayerSave.Gold -= iUnlockObject.UnlockCost;
        iUnlockObject.UnlockSuccessfully();
        AudioManager.Instance.PlayAudio(AudioType.ClickUpgrade);
        //Event firebase
        AnalyticsManager.LogEventGoldSpend(iUnlockObject.UnlockCost, "unlock", iUnlockObject.ToString());
    }

    private void OnGoldChange()
    {
        if (!inUnlockZone) return;
        LoadData();
    }

    private void LoadData()
    {
        EnoughGoldTextResponse(iUnlockObject.UnlockCost, unlockCostText);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        LoadData();
        canvasPopUp.gameObject.SetActive(true);
        inUnlockZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        canvasPopUp.OnClose();
        inUnlockZone = false;
    }
}
