using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pool
{
    public string poolName;
    public GameObject prefab;
    public int size;
    public bool expandable;

    public List<GameObject> itemList = new List<GameObject>();
}

public class SimplePool : MonoBehaviour
{
    private Pool pool;
    private Transform parent;

    protected virtual void Start()
    {
        parent = new GameObject(pool.prefab.name + " Parent").transform;
        Init();
    }

    protected virtual void Init()
    {
        for (int i = 0; i < pool.size; i++)
        {
            SpawnItem(pool);
        }
    }

    public virtual GameObject SpawnItem(Pool pool)
    {
        GameObject go = Instantiate(pool.prefab, parent, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localEulerAngles = Vector3.zero;
        go.SetActive(false);
        pool.itemList.Add(go);
        return go;
    }

    public GameObject GetItem(Pool pool)
    {
        for (int i = 0; i < pool.itemList.Count; i++)
        {
            if (!pool.itemList[i].activeInHierarchy)
            {
                return pool.itemList[i];
            }
        }
        if (pool.expandable)
        {
            return SpawnItem(pool);
        }
        else
        {
            return null;
        }
    }
}
