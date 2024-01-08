using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SimplePool
{
    public List<Pool> poolList;
    public Dictionary<string, Pool> poolDict;

    protected override void Init()
    {
        Debug.LogWarning("Init");
        poolDict = new Dictionary<string, Pool>();
        for (int i = 0; i < poolList.Count; i++)
        {
            for (int j = 0; j < poolList[i].size; j++)
            {
                SpawnItem(poolList[i]);
            }
            poolDict.Add(poolList[i].poolName, poolList[i]);
        }
    }

    public override GameObject SpawnItem(Pool pool)
    {
        return base.SpawnItem(pool);
    }

    public GameObject GetItem(string poolName)
    {
        poolDict.TryGetValue(poolName, out Pool pool);
        if (pool != null)
        {
            for (int i = 0; i < pool.itemList.Count; i++)
            {
                if (pool.itemList[i].activeInHierarchy)
                {
                    return pool.itemList[i];
                }
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
