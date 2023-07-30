using UnityEngine;
using static NVTT.Utilities;
public class CollectibleMagnet : MonoBehaviour
{
    public CurrencyType currencyType;
    public bool isMoving;
    public float moveSpeed;

    private void OnDisable()
    {
        isMoving = false;
    }

    private void Collect(float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, Player.transform.position +  Vector3.up * 0.3f, speed * Time.deltaTime);
    }
    private static bool IsInCollectRange(Transform target, Vector3 offset, float range)
    {
        return (Player.transform.position + offset - target.position).sqrMagnitude <= range * range;
    }

    private void AddCurrency()
    {
        switch (currencyType)
        {
            case CurrencyType.Gold:
                PlayerSave.Gold++;
                CollectibleCollector.CollectedGoldThisLevel++;
                break;
            case CurrencyType.Diamond:
                PlayerSave.Diamond++;
                CollectibleCollector.CollectedDiamondThisLevel++;
                break;
        }
    }
    
    private void Update()
    {
        if (!isMoving) return;
        Collect(moveSpeed);
        CollectibleCollector.Instance.collectibleList.Remove(this);

        if (!IsInCollectRange(transform, Vector3.up * 0.3f, 0.1f)) return;
        AudioManager.Instance.PlayAudio(AudioType.CollectSingleCoin);
        gameObject.SetActive(false);
        AddCurrency();
    }
}

public enum CurrencyType
{
    Gold,
    Diamond
}
