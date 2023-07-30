using AVT;
using DG.Tweening;
using UnityEngine;

public class CollectibleDrop : MonoBehaviour
{
    private Vector3 deltaPos;
    protected float RandomSpreadRange;
    protected int GoldsDropAmount;
    protected int DiamondsDropAmount;
    
    public void DropGold() => DropCurrency(GoldsDropAmount, ObjectPoolType.Gold);
    public void DropGold(Vector3 pos) => DropCurrency(pos, GoldsDropAmount, ObjectPoolType.Gold);
    public void DropDiamond() => DropCurrency(DiamondsDropAmount, ObjectPoolType.Diamond);
    public void DropDiamond(Vector3 pos) => DropCurrency(pos, DiamondsDropAmount, ObjectPoolType.Diamond);

    private void DropCurrency(int amount, int opType)
    {
        for (var i = 0; i < amount; i++)
        {
            var collectible = ObjectPooler.Instance.GetPooledObject(opType);
            var collectibleMagnet = collectible.GetComponent<CollectibleMagnet>();

            collectible.transform.position = transform.position + Vector3.up * 0.1f;
            collectible.SetActive(true);
            
            deltaPos = Random.insideUnitSphere * RandomSpreadRange;
            deltaPos.y = 0;
            
            collectible.transform.DoParabolaMotion(collectible.transform.position + deltaPos, 0.2f, 0.2f)
                .OnComplete(() => CollectibleCollector.Instance.collectibleList.Add(collectibleMagnet));
        }
    }

    private void DropCurrency(Vector3 pos, int amount, int opType)
    {
        for (var i = 0; i < amount; i++)
        {
            var collectible = ObjectPooler.Instance.GetPooledObject(opType);
            var collectibleMagnet = collectible.GetComponent<CollectibleMagnet>();

            collectible.transform.position = pos + Vector3.up * 0.1f;
            collectible.SetActive(true);
            
            deltaPos = Random.insideUnitSphere * RandomSpreadRange;
            deltaPos.y = 0;
            
            collectible.transform.DoParabolaMotion(collectible.transform.position + deltaPos, 0.2f, 0.2f)
                .OnComplete(() => CollectibleCollector.Instance.collectibleList.Add(collectibleMagnet));
        }
    }
}
