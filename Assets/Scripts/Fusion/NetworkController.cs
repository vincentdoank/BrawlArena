using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct NetworkInputData : INetworkInput
{
    public Vector2 moveDirection;
    public bool isJumpPressed;
    public bool isAttackPressed;
    public bool isSpecialAttackPressed;
    public bool isUltimateAttackPressed;
}


public class NetworkController : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private int playerCount = 0;

    private int connectedDeviceCount = 0;

    public bool isServer = false;

    public string errorMessage;

    public const ushort MAX_PLAYER = 1;

    private PlayerRef playerRef;

    public static NetworkController Instance { get; private set; }

    public int GetClientId()
    {
        return playerRef.PlayerId;
    }

    public int GetClientCount()
    {
        return _runner.ActivePlayers.Count();
    }

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        //EventManager.onGetIsConnected += GetIsConnected;
        //EventManager.onConnectToNetwork += Connect;
        //EventManager.onLeaveRoom += ExitRoom;

        if (isServer)
        {
            yield return new WaitForSeconds(2f);
            //Connect();
        }
        Debug.LogWarning("IsServer : " + isServer);
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.LogWarning("connected : " + clientId);
        EventManager.onReady?.Invoke(_runner.ActivePlayers.ToList().Count);

        //if (NetworkManager.Singleton.ConnectedClients.Count == MAX_PLAYER)
        //{
        //    GameManager.Instance.PrepareGame();
        //}
    }

    private void OnDestroy()
    {
        //EventManager.onConnectToNetwork -= Connect;
        EventManager.onLeaveRoom -= ExitRoom;
    }

    //private void Log(NetworkEvent eventType, ulong clientId, ArraySegment<byte> payload, float receiveTime)
    //{
    //    Debug.Log(eventType + " " + clientId);
    //}

    private bool GetIsConnected()
    {
        return _runner.IsConnectedToServer;
    }

    private void OnFailed()
    {
        errorMessage = "Failed";
        Debug.LogWarning("transport failed");
    }

    public void CreateRoom(string roomName)
    {
        StartGame(roomName, GameMode.Server);
    }

    public void JoinRoom(string roomName)
    {
        StartGame(roomName, GameMode.Client);
    }


    public void ExitRoom()
    {
        Debug.Log("ExitRoom");
        try
        {
            //NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId); //this is for kick a client
            //NetworkManager.Singleton.Shutdown(true);

            //PhotonNetwork.LeaveRoom(PhotonNetwork.IsMasterClient);
        }
        catch (Exception exc)
        {

        }
    }

    async void StartGame(string roomName, GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomName,
            Scene = scene,
            PlayerCount = 4,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new NetworkInputData();
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);
            playerCount += 1;
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
}
