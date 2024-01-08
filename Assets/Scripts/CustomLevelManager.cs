using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;
using MoreMountains.MMInterface;
using UnityEngine.Events;
using MoreMountains.CorgiEngine;
using WTI.NetCode;


public class CustomLevelManager : LevelManager
{
    public UnityEvent<AvatarId> onPlayerInstantiated;
    public UnityEvent onGameReady;


    public override void Start()
    {
        Debug.LogWarning("custom level manager started");
        base.Start();
    }

    public void SpawnPlayer()
    {
        Debug.LogWarning("instantiate playable characters");
        if (NetworkController.Instance.isServer)
        {
            foreach (Character player in Players)
            {
                onPlayerInstantiated?.Invoke(player.GetComponent<AvatarId>());
            }
        }
        //SpawnMultipleCharacters();
    }

    private IEnumerator WaitForReady()
    {
        yield return new WaitForSeconds(1f);
        onGameReady?.Invoke();
    }
}

