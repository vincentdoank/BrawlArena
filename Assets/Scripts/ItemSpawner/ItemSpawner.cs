using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemSpawner : PoolManager
{
    public List<ItemBooster> possibleItemList;

    public float firstSpawnDelay;
    public float spawnInterval;
    private ItemBooster spawnedItem;
    private Coroutine spawnRoutine;

    protected override void Start()
    {
        GameManager.Instance.onGameStarted += Spawn;
        GameManager.Instance.onGameNextRound += Spawn;
        Init();
    }

    private void OnDestroy()
    {
        GameManager.Instance.onGameStarted -= Spawn;
        GameManager.Instance.onGameNextRound -= Spawn;
    }

    private void Spawn()
    {
        spawnRoutine = StartCoroutine(WaitForSpawn());
    }

    private IEnumerator WaitForSpawn()
    {
        Debug.LogWarning("WaitForSpawn");
        yield return new WaitForSeconds(spawnInterval);
        Debug.LogWarning("WaitForSpawn Completed");
        int rand = Random.Range(0, possibleItemList.Count);
        spawnedItem = GetItem(poolList[rand].poolName).GetComponent<ItemBooster>();
        spawnedItem.gameObject.SetActive(true);
        spawnedItem.transform.SetParent(transform);
        spawnRoutine = null;
    }

    public void Release()
    {
        spawnedItem = null;
        Spawn();
    }
}
