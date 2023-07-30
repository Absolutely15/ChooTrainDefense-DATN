using Sirenix.OdinInspector;
using UnityBase.DesignPattern;
using UnityEngine;
using Random = UnityEngine.Random;

public class Barrier : MonoBehaviour
{
    #region Properties
    [Title("Reference")]
    public Health health;
    public GameObject model;
    [SerializeField] private GameObject canvasFix;
    [SerializeField] private BarrierData barrierData;
    
    [Title("Spawn Box")]
    [SerializeField] private bool showDebugBox;
    [SerializeField] private Vector3 halfSize;
    [SerializeField] private Transform spawnBox;
    
    private Vector3 randomPosInBox;
    private bool readyToFix;
    private readonly Color transBlueColor = new Color(0, 0, 1, 0.5f);
    #endregion

    #region Unity Functions
    private void Start()
    {
        InitObserver();
        GetData();
    }

    private void OnDrawGizmos()
    {
        if (!showDebugBox) return;
        Gizmos.color = transBlueColor;
        var position = spawnBox.position;
        Gizmos.DrawCube(position, halfSize * 2);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(position, halfSize * 2);
    }
    #endregion

    #region Zombie Spawn
    public Vector3 GetRandomPositionInsideBox()
    {
        randomPosInBox.Set(Random.Range(-halfSize.x, halfSize.x),0,Random.Range(-halfSize.z, halfSize.z));
        return spawnBox.position + randomPosInBox;
    }
    #endregion
    
    #region Upgrade Barrier

    private void InitObserver()
    {
        Observer.Instance.AddObserver(EventID.StartGameLevel, UnableToRebuild);
        Observer.Instance.AddObserver(EventID.EndGameLevel, AbleToRebuild);
        Observer.Instance.AddObserver(EventID.RebuildAllBarrier, RebuildBarrier);
        Observer.Instance.AddObserver(EventID.UpgradeBarrier, UpgradeBarrier);
    }
    
    private void UpgradeBarrier()
    {
        GetData();
        Observer.Instance.Notify(EventID.UpgradeDefense);
    }

    private void GetData()
    {
        if (readyToFix)
            health.SetMaxHealth(barrierData.healthUpgradeStat[PlayerSave.BarrierLevel]);
        else
            health.SetUpgradeHealth(barrierData.healthUpgradeStat[PlayerSave.BarrierLevel]);
        
    }
    #endregion
    
    #region Rebuild Barrier
    private void RebuildBarrier()
    {
        readyToFix = false;
        canvasFix.SetActive(false);
        health.ResetHealthWithModel();// sua sau
    }

    private void UnableToRebuild()
    {
        readyToFix = false;
        canvasFix.SetActive(false);
    }

    private void AbleToRebuild()
    {
        if (health.IsFullHealth()) return;
        readyToFix = true;
        canvasFix.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player") || !readyToFix) return;
        RebuildBarrier();
        Observer.Instance.Notify(EventID.RebuildBarrier);
    }
    #endregion
    
#if UNITY_EDITOR
    [Button]
    public void SetCanvasPos()
    {
        var rot = transform.localRotation.eulerAngles;
        var temp = new Vector3(rot.x, rot.y + 180, rot.z);
        canvasFix.transform.localRotation = Quaternion.Euler(temp);
    }
#endif
}
