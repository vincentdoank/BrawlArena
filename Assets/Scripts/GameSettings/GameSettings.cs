using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using MoreMountains.CorgiEngine;

public class GameSettings : MonoBehaviour
{

    public Dictionary<List<Transform>, float> collidingTimeDict = new Dictionary<List<Transform>, float>();
    public float repeatCollideDelay = 3f;

    public static GameSettings Instance { get; private set; }

    public virtual void StartGame(List<PlayerData> playerDataList)
    {

    }

    public virtual int GetDefaultScore()
    {
        return 0;
    }

    private void Start()
    {
        Instance = this;
    }

    public void CheckRepeatCollide(Transform player, Transform otherPlayer)
    {
        Collide(player, otherPlayer);
    }


    public virtual void Collide(Transform player, Transform otherPlayer)
    {
        
    }

    private void Update()
    {
        if (collidingTimeDict.Count > 0)
        {
            Dictionary<List<Transform>, float> tempDict = new Dictionary<List<Transform>, float>(collidingTimeDict);
            foreach (KeyValuePair<List<Transform>, float> kvp in tempDict)
            {
                collidingTimeDict[kvp.Key] += Time.deltaTime;
                if (collidingTimeDict[kvp.Key] >= repeatCollideDelay)
                {
                    collidingTimeDict.Remove(kvp.Key);
                }
            }
        }
    }

    public List<Transform> GetColliding(Transform player1, Transform player2)
    {
        return collidingTimeDict.Where(x => x.Key.Contains(player1) && x.Key.Contains(player2)).Select(x => x.Key)?.FirstOrDefault();
    }

    public void RemoveCollide(Transform player, Transform otherPlayer)
    {
        List<Transform> collidingEntities = GetColliding(player, otherPlayer);
        if (collidingEntities != null)
        {
            collidingTimeDict.Remove(collidingEntities);
        }
    }

    protected bool CompareLayer(LayerMask layerMask, int layer)
    {
        if ((layerMask & 1 << layer) == 1 << layer)
        {
            return true;
        }
        return false;
    }
}
