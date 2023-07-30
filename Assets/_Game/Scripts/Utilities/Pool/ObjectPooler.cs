using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand = true;

    public ObjectPoolItem(GameObject obj, int amt, bool exp = true)
    {
        objectToPool = obj;
        amountToPool = Mathf.Max(amt, 2);
        shouldExpand = exp;
    }
}

public static class ObjectPoolType
{
    public const int Gold = 0;
    public const int Diamond = 1;
}
public class ObjectPooler : Singleton<ObjectPooler>
{
    public List<ObjectPoolItem> itemsToPool;
    
    private List<GameObject> pooledObjects = new List<GameObject>();
    private readonly List<List<GameObject>> pooledObjectsList = new List<List<GameObject>>();
    private readonly List<int> positions = new List<int>();

    private int curSize;
    private int posIndex;
    private void Start()
    {
        for (var i = 0; i < itemsToPool.Count; i++)
        {
            ObjectPoolItemToPooledObject(i);
        }
    }


    public GameObject GetPooledObject(int index)
    {
        curSize = pooledObjectsList[index].Count;
        for (var i = positions[index] + 1; i < positions[index] + pooledObjectsList[index].Count; i++)
        {
            posIndex = i % curSize;
            if (pooledObjectsList[index][posIndex].activeInHierarchy) continue;
            positions[index] = posIndex;
            return pooledObjectsList[index][posIndex];
        }

        if (!itemsToPool[index].shouldExpand) return null;
        var obj = Instantiate(itemsToPool[index].objectToPool, transform, true);
        obj.SetActive(false);
        pooledObjectsList[index].Add(obj);
        return obj;
    }

    public List<GameObject> GetAllPooledObjects(int index)
    {
        return pooledObjectsList[index];
    }


    public int AddObject(GameObject GO, int amt = 3, bool exp = true)
    {
        var item = new ObjectPoolItem(GO, amt, exp);
        var currLen = itemsToPool.Count;
        itemsToPool.Add(item);
        ObjectPoolItemToPooledObject(currLen);
        return currLen;
    }


    private void ObjectPoolItemToPooledObject(int index)
    {
        var item = itemsToPool[index];

        pooledObjects = new List<GameObject>();
        for (var i = 0; i < item.amountToPool; i++)
        {
            var obj = Instantiate(item.objectToPool, transform, true);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }

        pooledObjectsList.Add(pooledObjects);
        positions.Add(0);
    }
}