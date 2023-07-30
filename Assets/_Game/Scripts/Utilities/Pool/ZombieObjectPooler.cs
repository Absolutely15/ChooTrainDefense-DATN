using System.Collections.Generic;


[System.Serializable]
public class ObjectPoolZombie
{
    public Zombie objectToPool;
    public int amountToPool;
    public bool shouldExpand = true;
}
public class ZombieObjectPooler : Singleton<ZombieObjectPooler>
{
    public List<ObjectPoolZombie> zombiesToPool;
    
    private List<Zombie> pooledObjects = new List<Zombie>();
    private readonly List<List<Zombie>> pooledObjectsList = new List<List<Zombie>>();
    private readonly List<int> positions = new List<int>();

    private int curSize;
    private int posIndex;
    private void Start()
    {
        for (var i = 0; i < zombiesToPool.Count; i++)
        {
            ObjectPoolZombieToPooledObject(i);
        }
    }
    
    public Zombie GetPooledObject(int index)
    {
        curSize = pooledObjectsList[index].Count;
        for (var i = positions[index] + 1; i < positions[index] + pooledObjectsList[index].Count; i++)
        {
            posIndex = i % curSize;
            if (pooledObjectsList[index][posIndex].gameObject.activeInHierarchy) continue;
            positions[index] = posIndex;
            return pooledObjectsList[index][posIndex];
        }

        if (!zombiesToPool[index].shouldExpand) return null;
        var obj = Instantiate(zombiesToPool[index].objectToPool, transform, true);
        obj.gameObject.SetActive(false);
        pooledObjectsList[index].Add(obj);
        return obj;
    }
    
    private void ObjectPoolZombieToPooledObject(int index)
    {
        var item = zombiesToPool[index];

        pooledObjects = new List<Zombie>();
        for (var i = 0; i < item.amountToPool; i++)
        {
            var obj = Instantiate(item.objectToPool, transform, true);
            obj.gameObject.SetActive(false);
            pooledObjects.Add(obj);
        }

        pooledObjectsList.Add(pooledObjects);
        positions.Add(0);
    }
}
