using MoreMountains.CorgiEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPotatoSettings : GameSettings
{
    public HotPotatoSettingsScriptableObject gameSettings;
    public ParticleSystem explosionFx;

    public override int GetDefaultScore()
    {
        return gameSettings.lives;
    }

    public float GetBombDuration()
    {
        return Random.Range(gameSettings.minBombDuration, gameSettings.maxBombDuration);
    }

    private void SendPlayerDead(ulong clientId)
    {
        EventManager.onPlayerDead?.Invoke(clientId);
    }

    public void Explode(Character character)
    {
        character.GetComponent<CharacterAttributes>().OnBombReleased();
        PlayExplosion(character);
        CustomLevelManager.Instance.KillPlayer(character);
        string id = character.GetComponent<AvatarId>()?.id;
        ScoreController.Instance.SubScore(id, (score) =>
        {
            SendPlayerDead(GameManager.Instance.GetClientId(id));
            StartCoroutine(TimeController.Instance.Wait(2f, () => GameManager.Instance.Respawn(new List<int>())));
            GameManager.Instance.Reset();
        });
    }

    public override void StartGame(List<PlayerData> playerDataList)
    {
        if (CheckGame(playerDataList))
        {
            base.StartGame(playerDataList);
            StartCoroutine(WaitForGameStart(playerDataList));
        }
        else
        {
            Debug.LogWarning("end");
            ApplyBomb(null);
        }
    }

    public bool CheckGame(List<PlayerData> playerDataList)
    {
        if (playerDataList.Where(x => x.score > 0).Count() == 1)
        {
            return false;
        }
        return true;
    }

    public IEnumerator WaitForGameStart(List<PlayerData> playerDataList)
    {
        //BombTimer.Instance.Reset();
        yield return new WaitForSeconds(2f);
        //Transform target = GetRandomPlayer(playerDataList);

        Transform target = GetRandomPlayer(playerDataList, out int index);
        SpinningWheel.Instance.Spin(playerDataList, index, () =>
        {
            //ApplyBomb(target);
            SpinningWheel.Instance.Throw(target, () => StartGame(target));
        });
    }

    private void StartGame(Transform target)
    {
        ApplyBomb(target);
        GameManager.Instance.onGameStarted?.Invoke();
    }

    private Transform GetRandomPlayer(List<PlayerData> playerDataList, out int index)
    {
        List<PlayerData> filteredPlayerDataList = playerDataList.Where(x => x.score > 0).ToList();
        int rand = Random.Range(0, filteredPlayerDataList.Count);
        index = rand;
        //Debug.LogWarning("score : " + filteredPlayerDataList[rand].score + " " + filteredPlayerDataList[rand].playerId);
        Debug.LogWarning("result : " + filteredPlayerDataList[rand].playerId + " " + index);
        if (filteredPlayerDataList[rand].score <= 0)
        {
            return GetRandomPlayer(playerDataList, out index);
        }
        return filteredPlayerDataList[rand].entity;
    }

    private int GetRandomPlayerIndex(List<PlayerData> playerDataList)
    {
        List<PlayerData> filteredPlayerDataList = playerDataList.Where(x => x.score > 0).ToList();
        int rand = Random.Range(0, filteredPlayerDataList.Count);
        return rand;
    }

    private void ApplyBomb(Transform target)
    {
        GameManager.Instance.StartGame();
        BombTimer.Instance.SetTarget(target);
        target.GetComponent<CharacterAttributes>().OnBombGrabbed();
    }

    private void SwitchBomb(Transform player1, Transform player2)
    {
        Debug.Log("switch bomb : " + player1 + " " + player2);
        BombTimer.Instance.SetTarget(player1, player2);
    }

    public void PlayExplosion(Character character)
    {
        explosionFx.transform.position = character.transform.position;
        explosionFx.gameObject.SetActive(true);
        explosionFx.Play(true);
    }

    public override void Collide(Transform player, Transform otherPlayer)
    {
        base.Collide(player, otherPlayer);
        if (CompareLayer(GameManager.Instance.playerLayerMask, otherPlayer.gameObject.layer))
        {
            Debug.Log("collide : " + player + " " + otherPlayer);
            if (GetColliding(player, otherPlayer) == null)
            {
                collidingTimeDict.Add(new List<Transform> { player, otherPlayer }, 0);
                SwitchBomb(player, otherPlayer);
            }

            Debug.Log("collidingTimedict : " + collidingTimeDict.Count);
            foreach (KeyValuePair<List<Transform>, float> kvp in collidingTimeDict)
            {
                string key = "item : ";
                for (int i = 0; i < kvp.Key.Count; i++)
                {
                    key += kvp.Key[i].name + " ";
                }
                Debug.Log(key);
            }
        }
    }
}
