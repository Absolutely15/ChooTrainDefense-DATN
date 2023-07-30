using System.Collections.Generic;
using UnityEngine;

public class SimplePool<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Properties

    public T prefab;
    public int preSpawn;

    private List<T> pool = new List<T>();

    #endregion
    
    #region Query

    public int Count => pool.Count;

    #endregion

    #region Interface

    protected virtual void Init(T obj)
    {
        
    }

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        for (var i = 0; i < preSpawn; i++)
        {
            var clone = Instantiate(prefab, transform);
            clone.gameObject.SetActive(false);
            Supply(clone);
        }
    }

    #endregion

    #region API
    
    public void Supply(T item, bool isChildOfPool = false)
    {
        Init(item);
        pool.Add(item);
        
        if (isChildOfPool)
            item.transform.SetParent(transform);
    }

    public T Get()
    {
        foreach (var p  in pool)
        {
            if (p.gameObject.activeInHierarchy == false)
                return p;
        }
        return pool[0];
    }
    #endregion
}
