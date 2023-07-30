using System;
using DG.Tweening;
using UnityEngine;
using NVTT;
using Sirenix.OdinInspector;
using UnityBase.DesignPattern;
using UnityEngine.UI;
using static NVTT.Utilities;

public class GameTutorial : MonoBehaviour
{
    #region Properties
    public bool skipTutorial;
    
    [Title("References")]
    [SerializeField] private RectTransform arrowGuideStart;
    [SerializeField] private GameObject transparentPanel;
    
    [Title("Drag To Move")]
    [SerializeField] private GameObject dragToMove;

    [Title("Unlock Room")]
    [SerializeField] private GameObject camTut;
    [SerializeField] private GameObject unlockRoomUITut;
    
    [Title("Upgrade Gun")]
    [SerializeField] private GameObject arrowUpgradeArea;
    [SerializeField] private GameObject arrowUpgradeBtn;

    [Title("Rebuild Barrier")]
    [SerializeField] private GameObject rebuildBarrierUITut;

    private bool isEndGameLevelShow;
    
    private Button startBtn;
    private UpgradeInMap upgradeInMap;
    private UpgradeGunInMap upgradeGunUI;
    #endregion

    #region Keys
    private const string DRAG_TO_MOVE_TUTORIAL = "drag_to_move_tutorial";
    private const string UNLOCK_ROOM_TUTORIAL = "unlock_room_tutorial";
    private const string UPGRADE_GUN_TUTORIAL = "upgrade_gun_tutorial";
    private const string REBUILD_TUTORIAL = "rebuild_tutorial";
    private const string START_TUTORIAL = "start_tutorial";
    #endregion

    #region Init
    private void Start()
    {
        if (skipTutorial) return;

        startBtn = UIManager.Instance.playBtn.GetComponent<Button>();

        DragToMoveTutorial();
        UnlockRoomTutorial();
        UpgradeGunTutorial();
        SetInventoryBtnActive();
        
        if (IsPassBasicTutorial) return;
        UIManager.Instance.shopBtn.SetActive(false);
        Gameplay.onEndGame.AddListener(RebuildBarrierTutorial);
        Observer.Instance.AddObserver(EventID.StartGameLevel, OnStartGame);
        Observer.Instance.AddObserver(EventID.EndGameLevelData, OnEndGameData);
        Observer.Instance.AddObserver(EventID.InUpgradeZone, InUpgradeZone);
    }

    private void OnStartGame() => isEndGameLevelShow = false;

    private void OnEndGameData()
    {
        isEndGameLevelShow = true;
        if (IsPassBasicTutorial) UIManager.Instance.shopBtn.SetActive(true);
    }

    private void InUpgradeZone()
    {
        if (IsPassBasicTutorial) return;
        if (!arrowGuideStart.gameObject.activeInHierarchy) return;
        arrowGuideStart.gameObject.SetActive(false);
    }
    #endregion

    #region Drag To Move
    private void DragToMoveTutorial()
    {
        if (DidTutorial(DRAG_TO_MOVE_TUTORIAL))
            return;
        
        dragToMove.SetActive(true);
        startBtn.gameObject.SetActive(false);
        Utilities.MapManager.blocker[0].blockerUI.SetBlockerStatus(false);
        
        Player.isMoving.AddListener(CompletedDragToMoveTut);
    }

    private void CompletedDragToMoveTut()
    {
        Player.isMoving.RemoveListener(CompletedDragToMoveTut);
        dragToMove.SetActive(false);
        TutorialArrowStart(() => Gameplay.onEndGame.AddListener(() => DOVirtual.DelayedCall(2, () => CompleteTutOnEndGameLevel(1, DRAG_TO_MOVE_TUTORIAL, UnlockRoomTutorial))));
    }
    #endregion

    #region Unlock Room
    private void UnlockRoomTutorial() // unlock room
    {
        if (DidTutorial(UNLOCK_ROOM_TUTORIAL) || !DidTutorial(DRAG_TO_MOVE_TUTORIAL))
            return;
        
        var blockerDefault = Utilities.MapManager.blocker[0];
        blockerDefault.onUnlockedRoom.AddListener(CompletedUnlockRoomTut);
        startBtn.gameObject.SetActive(false);
        
        DOVirtual.DelayedCall(2f, () =>
        {
            transparentPanel.SetActive(true);
            unlockRoomUITut.SetActive(true);
            camTut.SetActive(true);
            blockerDefault.blockerUI.SetBlockerStatus(true);
            Gameplay.player.DisableControl();

            DOVirtual.DelayedCall(2, () =>
            {
                transparentPanel.SetActive(false);
                camTut.SetActive(false);
                Gameplay.player.EnableControl();
            });
        });
    }
    
    private void CompletedUnlockRoomTut(Blocker blocker)
    {
        Utilities.MapManager.blocker[0].onUnlockedRoom.RemoveListener(CompletedUnlockRoomTut);
        DoCompleteTutorial(UNLOCK_ROOM_TUTORIAL);
        unlockRoomUITut.SetActive(false);
        UpgradeGunTutorial();
    }
    #endregion

    #region Upgrade Gun
    private void UpgradeGunTutorial()
    {
        if (DidTutorial(UPGRADE_GUN_TUTORIAL) || !DidTutorial(UNLOCK_ROOM_TUTORIAL))
            return;
        
        Observer.Instance.AddObserver(EventID.InUpgradeZone, InUpgradeZoneTut);
        startBtn.gameObject.SetActive(false);
        arrowUpgradeArea.SetActive(true);
    }

    private void InUpgradeZoneTut()
    {
        upgradeGunUI = Utilities.MapManager.GetComponentInChildren<UpgradeGunInMap>();
        upgradeGunUI.upgradeDamageBtn.upgradeBtn.onClick.AddListener(CompletedUpgradeGunTut);
        arrowUpgradeArea.SetActive(false);
        arrowUpgradeBtn.gameObject.SetActive(true);
        transparentPanel.SetActive(true);
        Player.DisableControl();
    }

    private void CompletedUpgradeGunTut()
    {
        upgradeGunUI.upgradeDamageBtn.upgradeBtn.onClick.RemoveListener(CompletedUpgradeGunTut);
        upgradeInMap = Utilities.MapManager.GetComponentInChildren<UpgradeInMap>();
        upgradeInMap.groupBtn.OnClose();
        Observer.Instance.RemoveObserver(EventID.InUpgradeZone, InUpgradeZoneTut);
        arrowUpgradeBtn.gameObject.SetActive(false);
        transparentPanel.SetActive(false); 
        Player.EnableControl();
        DoCompleteTutorial(UPGRADE_GUN_TUTORIAL);
        TutorialArrowStart(null, !rebuildBarrierUITut.activeInHierarchy);
    }

    #endregion

    #region Rebuild Barrier
    private void RebuildBarrierTutorial()
    {
        if (DidTutorial(REBUILD_TUTORIAL) || Utilities.MapManager.barrierListSpawn[0].health.IsFullHealth()) return;
        Gameplay.onEndGame.RemoveListener(RebuildBarrierTutorial);
        Observer.Instance.AddObserver(EventID.RebuildBarrier, CompletedRebuildBarrierTut);
        startBtn.gameObject.SetActive(false);
        rebuildBarrierUITut.SetActive(true);
        UIManager.Instance.isDoingBarrierTut = true;
    }

    private void CompletedRebuildBarrierTut()
    {
        rebuildBarrierUITut.SetActive(false);
        UIManager.Instance.isDoingBarrierTut = false;
        DoCompleteTutorial(REBUILD_TUTORIAL);
        TutorialArrowStart(() => Observer.Instance.RemoveObserver(EventID.RebuildBarrier, CompletedRebuildBarrierTut),
            DidTutorial(UNLOCK_ROOM_TUTORIAL) && DidTutorial(UPGRADE_GUN_TUTORIAL) && isEndGameLevelShow);
    }
    #endregion

    #region Inventory Button

    private static void SetInventoryBtnActive()
    {
        if (Utilities.MapManager.room[2].activeInHierarchy)
            InventoryBtnActive(true);
        else
        {
            InventoryBtnActive(false);
            Utilities.MapManager.blocker[1].onUnlockedRoom.AddListener(SetInventoryBtnOnUnlockRoom);
        }
    }

    private static void SetInventoryBtnOnUnlockRoom(Blocker blocker)
    {
        Utilities.MapManager.blocker[1].onUnlockedRoom.RemoveListener(SetInventoryBtnOnUnlockRoom);
        InventoryBtnActive(true);
    }

    private static void InventoryBtnActive(bool status) => UIManager.Instance.inventoryBtn.SetActive(status);
    
    #endregion

    #region Utils
    private static bool DidTutorial(string key) => PlayerPrefs.GetInt(key, 0) == 1;
    private static void DoCompleteTutorial(string key) => PlayerPrefs.SetInt(key, 1);

    private static void CompleteTutOnEndGameLevel(int level, string key, Action action)
    {
        if (PlayerSave.CurrentGameLevel != level) return;
        Gameplay.onEndGame.RemoveListener(() => CompleteTutOnEndGameLevel(level, key, action));
        DoCompleteTutorial(key);
        action();
    }
    
    private void TutorialArrowStart(Action action, bool isActive = true, bool completed = false)
    {
        if (DidTutorial(START_TUTORIAL))
            return;

        arrowGuideStart.gameObject.SetActive(isActive);
        startBtn.gameObject.SetActive(isActive);
        startBtn.onClick.AddListener(() => StartBtnOnClick(action, completed));
    }

    private void StartBtnOnClick(Action action, bool completed = false)
    {
        startBtn.onClick.RemoveListener(() => StartBtnOnClick(action, completed));
        arrowGuideStart.gameObject.SetActive(false);
        action?.Invoke();
        if (!completed) return;
        DoCompleteTutorial(START_TUTORIAL);
    }
    #endregion

}
