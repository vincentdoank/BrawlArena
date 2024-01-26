using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

[Serializable]
public class PlayerData
{
    public string playerId;
    public Transform entity;
    public int score;
    public int rank;
    public ulong? occupantClientId = null;
    public bool isReady;
}

[Serializable]
public class AvatarSprite
{
    public string avatarId;
    public Sprite sprite;
}

public class GameManager : MonoBehaviour
{
    private Dictionary<ulong, string> clientDict = new Dictionary<ulong, string>();
    private Dictionary<ulong, Character> clientCharacterDict = new Dictionary<ulong, Character>();

    public GameSettings gameSettings;
    public GameObject avatarProfilePrefab;
    public Transform avatarProfileParent;

    public List<PlayerData> playerDataList;
    public List<AvatarSprite> avatarSpriteList;

    public GameObject playerPanel;
    public GameObject serverIdlePanel;

    public GameObject playerObject;
    public GameObject serverObject;

    public GameObject serverUI;
    public GameObject playerUI;

    public Button restartButton;

    public Camera gameCamera;

    public Dictionary<string, AvatarProfile> avatarProfileDict = new Dictionary<string, AvatarProfile>();

    public LayerMask playerLayerMask;
    public Action onGameStarted;
    public Action onGameNextRound;

    private bool isGameRunning;

    public static GameManager Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        EventManager.onReady += OnPlayerReady;
        EventManager.onDisconnected += OnDisconnected;
        restartButton.onClick.AddListener(RestartGame);
        Debug.LogWarning("Check IsServer : " + NetworkController.Instance.isServer.ToString());
        ShowPlayerIdleUI(!NetworkController.Instance.isServer);
        ShowServerIdleUI(NetworkController.Instance.isServer);
        ((CustomLevelManager)LevelManager.Instance).onPlayerInstantiated.AddListener(AddPlayer);
        //StartCoroutine(Test());
    }

    private void RestartGame()
    {
        //FIX
        //PlayerData data = playerDataList.Where(x => x.occupantClientId == NetworkController.Instance.GetClientId()).FirstOrDefault();
        //data.isReady = true;
        //EventManager.onReady?.Invoke(playerDataList.Where(x => x.isReady).Count());
        EventManager.onPlayerReady?.Invoke();
        restartButton.gameObject.SetActive(false);
    }

    private void ResetGame()
    {
        gameCamera.gameObject.SetActive(true);
    }

    //private IEnumerator Test()
    //{
    //    UnityWebRequest uwr = new UnityWebRequest("https://api.openweathermap.org/data/2.5/weather?lat=16.1304&lon=60.3468&appid=2e1d07536db7b31f962c70b6664485b0", "GET");
    //    uwr.downloadHandler = new DownloadHandlerBuffer();
    //    yield return uwr.SendWebRequest();
    //    if (uwr.result == UnityWebRequest.Result.Success)
    //    {
    //        //if (uwr.downloadHandler != null)
    //        {
    //            byte[] test = uwr.downloadHandler.data;
    //            for (int i = 0; i < test.Length; i++)
    //            {
    //                Debug.Log("test : "+ test[i]);
    //            }
    //            Debug.Log("res : " + uwr.downloadHandler.text);
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("err : " + uwr.responseCode);
    //    }
    //}

    private void OnDestroy()
    {
        EventManager.onReady -= OnPlayerReady;
        EventManager.onDisconnected -= OnDisconnected;
    }

    public void OnServerConnected()
    {
        serverObject.SetActive(true);
        playerObject.SetActive(false);
    }

    public void OnClientConnected()
    {
        serverObject.SetActive(false);
        playerObject.SetActive(true);
    }

    public void InitGame()
    {
        if (NetworkController.Instance.isServer)
        {
            ((CustomLevelManager)LevelManager.Instance).SpawnPlayer();
        }
    }
    public void HidePreGameUI()
    {
        ShowPlayerIdleUI(false);
        ShowServerIdleUI(false);
        if (NetworkController.Instance.isServer)
        {
            ShowPlayerUI(false);
            ShowServerUI(true);
        }
        else
        {
            ShowPlayerUI(true);
            ShowServerUI(false);
        }
    }

    public void PrepareGame()
    {
        //GameSettings.Instance.StartGame(playerDataList);
        HidePreGameUI();

        if (NetworkController.Instance.isServer)
        {
            EventManager.onGameStarted?.Invoke();
        }
        Respawn();
        StopPlayers(true);
        ResetPlayersState();
    }

    public void ResetPlayersState()
    {
        for (int i = 0; i < playerDataList.Count; i++)
        {
            playerDataList[i].isReady = false;
        }
    }

    public void OnPlayerConnected(ulong clientId)
    {
        Debug.LogWarning("on player connected : " + clientId);
        if (NetworkController.Instance.isServer)
        {
            PlayerData data = GetAvailableCharacter(out int index);
            if (data != null)
            {
                clientCharacterDict.Add(clientId, data.entity.GetComponent<Character>());
                Debug.Log("clientcharacterdict count : " + clientCharacterDict.Count);

                foreach (KeyValuePair<ulong, Character> kvp in clientCharacterDict)
                {
                    Debug.Log("kvp : " + kvp.Key + " " + kvp.Value.name);
                }
                clientDict.Add(clientId, data.playerId);
                data.occupantClientId = clientId;
                playerDataList[index] = data;
            }
        }
    }

    private void OnDisconnected(ulong clientId)
    {
        Debug.LogWarning("disconnect " + clientId);
        if (clientDict.ContainsKey(clientId))
        {
            RemoveCharacter(clientId);
            clientDict.Remove(clientId);
            clientCharacterDict.Remove(clientId);
        }
    }

    private Character GetCharacter(ulong clientId)
    {
        if (clientCharacterDict.ContainsKey(clientId))
        {
            return clientCharacterDict[clientId];
        }
        else
        {
            Debug.LogError("character not found");
        }
        return null;
    }

    public ulong GetClientId(string avatarId)
    {
        if (clientDict.ContainsValue(avatarId))
        {
            return clientDict.Where(x => x.Value == avatarId).Select(x => x.Key).FirstOrDefault();
        }
        else
        {
            Debug.LogError("character not found");
        }
        return 0;
    }

    private void RemoveCharacter(ulong clientId)
    {
        PlayerData data = GetCharacterData(clientId);
        data.occupantClientId = null;
    }

    private PlayerData GetCharacterData(ulong clientId)
    {
        return playerDataList.Where(x => x.occupantClientId == clientId).FirstOrDefault();
    }

    private PlayerData GetAvailableCharacter(out int index)
    {
        index = playerDataList.FindIndex(0, playerDataList.Count, x => x.occupantClientId == null);
        Debug.Log("GetAvailableCharacter : " + playerDataList[index] == null ? "" : playerDataList[index].playerId);
        return playerDataList[index];
    }

    private void ShowPlayerIdleUI(bool value)
    {
        playerPanel.SetActive(value);
    }

    private void ShowPlayerUI(bool value)
    {
        playerUI.SetActive(value);
    }

    private void ShowServerIdleUI(bool value)
    {
        serverIdlePanel.SetActive(value);
    }

    private void ShowServerUI(bool value)
    {
        serverUI.SetActive(value);
    }

    public void Connect()
    {
        //NetworkController.Instance.Connect();
    }

    public void OnPlayerReady(int count)
    {
        //FIX THISSSSSS

        if (count == NetworkController.MAX_PLAYER)
        {
            PrepareGame();
        }
    }

    public void MoveCharacter(ulong clientId, Vector2 direction)
    {
        if (!isGameRunning) return;
        Character character = GetCharacter(clientId);
        if (character)
        {
            character.GetComponent<CharacterHorizontalMovement>().SetHorizontalMove(direction.x);
            character.GetComponent<CustomCharacterJump>().SetVerticalMove(direction.y);
        }
    }

    public void StartJumpCharacter(ulong clientId)
    {
        if (!isGameRunning) return;
        Debug.Log("start jump");
        Character character = GetCharacter(clientId);
        if (character)
        {
            character.GetComponent<CharacterJump>().JumpStart();
        }
    }

    public void StopJumpCharacter(ulong clientId)
    {
        if (!isGameRunning) return;
        Debug.Log("stop jump");
        Character character = GetCharacter(clientId);
        if (character)
        {
            character.GetComponent<CharacterJump>().JumpStop();
        }
    }

    public void Reset()
    {
        StopPlayers(true);
        //MoreMountains.CorgiEngine.GameManager.Instance.Paused = true;
        StartCoroutine(TimeController.Instance.Wait(1f, () => Respawn()));
    }

    public void StopPlayers(bool value)
    {
        //StartCoroutine(WaitToResetAbility(value));
        StartCoroutine(WaitPlayer(value));
    }

    private IEnumerator WaitPlayer(bool value)
    {
        yield return null;
        isGameRunning = !value;
        foreach (Character character in LevelManager.Instance.Players)
        {
            character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
            character.GetComponent<CustomCharacterHorizontalMovement>().ResetSpeed();
            character.Reset();
            character.ResetInput();


            foreach (CharacterAbility ability in character.GetComponents<CharacterAbility>())
            {
                ability.PermitAbility(!value);
                ability.ResetAbility();
            }
        }
    }

    private IEnumerator WaitToResetAbility(bool value)
    {
        foreach (Character character in LevelManager.Instance.Players)
        {
            character.Reset();
            character.ResetInput();
            yield return null;
            foreach (CharacterAbility ability in character.GetComponents<CharacterAbility>())
            {
                ability.PermitAbility(!value);
                ability.ResetAbility();
            }
            //if (!value)
            //{
            //    character.AssignAnimator();
            //}
        }
    }


    public void StartGame()
    {
        StopPlayers(false);
        ResultScreen.Instance.HideCamera();
        BombTimer.Instance.gameObject.SetActive(true);
    }

    //public void Explode(Character character)
    //{
    //    PlayExplosion(character);
    //    CustomLevelManager.Instance.KillPlayer(character);
    //}
    public void Respawn()
    {
        if (NetworkController.Instance.isServer)
        {
            Debug.LogError("start game");
            GameSettings.Instance.StartGame(playerDataList);
            if (playerDataList.Where(x => x.score > 0).Count() > 1)
            {
                SpinningWheel.Instance.Spawn(playerDataList);
            }
            else
            {
                gameCamera.gameObject.SetActive(false);
                ResultScreen.Instance.Init(playerDataList.OrderBy(x => x.rank).Select(x => x.playerId).ToArray());
                EventManager.onGameEnded?.Invoke();
            }
        }
    }

    public void Respawn(List<int> occupiedSpawnPointIndexList)
    {
        List<PlayerData> filteredPlayerDataList = playerDataList.Where(x => x.score > 0).ToList();
        for (int i = 0; i < filteredPlayerDataList.Count; i++)
        {
            //CustomLevelManager.Instance.Checkpoints[i].SpawnPlayer(character);

            int randPos = GetRandomSpawnPointIndex();
            while (occupiedSpawnPointIndexList.Contains(randPos))
            {
                randPos = GetRandomSpawnPointIndex();
            }
            occupiedSpawnPointIndexList.Add(randPos);
            CustomLevelManager.Instance.Checkpoints[randPos].SpawnPlayer(filteredPlayerDataList[i].entity.GetComponent<Character>());
        }
    }

    private int GetRandomSpawnPointIndex()
    {
        return Random.Range(0, CustomLevelManager.Instance.Checkpoints.Count);
    }

    public void AddPlayer(AvatarId avatarId)
    {
        Debug.LogWarning("add player : " + avatarId);
        int score = gameSettings.GetDefaultScore();
        playerDataList.Add(new PlayerData { playerId = avatarId.id, entity = avatarId.transform, score = score });
        SpawnAvatarProfile(avatarId.id, score);
        Debug.LogWarning("add player : " + playerDataList[playerDataList.Count - 1].playerId);
    }

    private void SpawnAvatarProfile(string avatarId, int score)
    {
        GameObject go = Instantiate(avatarProfilePrefab, avatarProfileParent, false);
        AvatarProfile avatarProfile = go.GetComponent<AvatarProfile>();
        avatarProfile.SetAvatar(GetAvatarSprite(avatarId));
        avatarProfile.SetScore(score);
        avatarProfileDict.Add(avatarId, avatarProfile);
    }

    public void SetScore(string avatarId, int score)
    {
        avatarProfileDict[avatarId].scoreText.text = score.ToString();
    }

    public Sprite GetAvatarSprite(string avatarId)
    {
        Sprite sprite = avatarSpriteList.Where(x => x.avatarId == avatarId).Select(x => x.sprite).FirstOrDefault();
        return sprite;
    }

    public void ShowRestartButton()
    {
        restartButton.gameObject.SetActive(true);
    }

    private void AssignController()
    {
        
    }

    public void PlayerDead()
    {
        Handheld.Vibrate();
    }

    public void PlayerReady(ulong clientId)
    {
        int index = playerDataList.FindIndex(0, playerDataList.Count, x => x.occupantClientId == clientId);
        playerDataList[index].isReady = true;
        EventManager.onReady?.Invoke(playerDataList.Where(x => x.isReady).Count());
    }
}
