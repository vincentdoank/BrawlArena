using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ScoreController : MonoBehaviour
{
    public static ScoreController Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    public virtual int AddScore(string playerId, Action<int> onScoreUpdated)
    {
        PlayerData data = GameManager.Instance.playerDataList.Where(x => x.playerId == playerId).FirstOrDefault();
        data.score += 1;
        GameManager.Instance.SetScore(playerId, data.score);
        onScoreUpdated?.Invoke(data.score);
        return data.score;
    }

    public virtual int SubScore(string playerId, Action<int> onScoreUpdated)
    {
        Debug.Log("playerId : " + playerId);
        for (int i = 0; i < GameManager.Instance.playerDataList.Count; i++)
        {
            Debug.Log("list id : " + GameManager.Instance.playerDataList[i].playerId);
        }
        PlayerData data = GameManager.Instance.playerDataList.Where(x => x.playerId == playerId).FirstOrDefault();
        data.score -= 1;
        if (data.score == 0)
        {
            data.rank = 4 - (GameManager.Instance.playerDataList.Where(x => x.score <= 0).Count() - 1);
        }
        GameManager.Instance.SetScore(playerId, data.score);
        onScoreUpdated?.Invoke(data.score);
        return data.score;
    }
}
