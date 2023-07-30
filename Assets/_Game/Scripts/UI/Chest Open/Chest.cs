using ATSoft.Ads;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    #region Properties
    [Title("State")]
    public bool isOpened;
    public int id;
    
    [Title("Event")]
    public UnityEvent<Chest> onOpen = new UnityEvent<Chest>();
    
    [Title("UI")]
    [SerializeField] private GameObject canvasOverlay;
    [SerializeField] private DotweenScale canvasPopUp;
    [SerializeField] private Button openBtn;
    [SerializeField] private Button claimBtn;

    [Title("Anim")]
    [SerializeField] private ChestAnim chestAnim;
    private IChest iChest;
    #endregion

    #region Unity Functions
    private void Start()
    {
        gameObject.SetActive(!isOpened);

        if (isOpened) return;
        iChest = GetComponent<IChest>();
        iChest.Init();
        openBtn.onClick.AddListener(OpenBtnCase);
        claimBtn.onClick.AddListener(ClaimBtnFunction);
        
        canvasPopUp.gameObject.SetActive(false);
    }
    #endregion

    #region On Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        canvasPopUp.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        canvasPopUp.OnClose();
    }
    #endregion

    #region Basic Functions
    private void OpenBtnCase()
    {
        if (iChest.ChestType == ChestType.DroneChest)
        {
            OpenBtnFunction();
            return;
        }
        
        UnityAction<bool> actionComplete = delegate(bool isSuccess)
        {
            OpenBtnFunction();
        };
        Advertisements.Instance.ShowRewardedVideo(actionComplete, $"Open{iChest.ChestType}");
    }
    private void OpenBtnFunction()
    {
        iChest.Open();
        GPExecutor.Instance.PauseGame();
        chestAnim.OpenChestResponse();
        canvasOverlay.SetActive(true);
        onOpen.Invoke(this);
    }

    private void ClaimBtnFunction()
    {
        iChest.Claim();
        GPExecutor.Instance.ResumeGame();
        gameObject.SetActive(false);
    }
    #endregion
}
