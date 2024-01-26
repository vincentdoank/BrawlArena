using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using Fusion;

namespace WTI.Fusion
{

    /// <summary>
    /// FOR NETCODE
    /// </summary>
    public class NetworkController33 : MonoBehaviour
    {
        //private int connectedDeviceCount = 0;

        //public bool isServer = false;

        //public string errorMessage;

        //public const ushort MAX_PLAYER = 1;

        //private PlayerRef playerRef;

        //public static NetworkController Instance { get; private set; }

        //public int GetClientId()
        //{
        //    return playerRef.PlayerId;
        //}

        //public int GetClientCount()
        //{
        //    return NetworkManager.Singleton.ConnectedClients.Count;
        //}

        //private void Awake()
        //{
        //    Instance = this;
        //    if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsPlayer)
        //    {
        //        isServer = false;
        //    }
        //    else
        //    {
        //        isServer = true;
        //    }
        //}

        //private IEnumerator Start()
        //{
            

        //    if (isServer)
        //    {
        //        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        //    }
        //    NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        //    NetworkManager.Singleton.ConnectionApprovalCallback += OnCheckApprovalConnection;
        //    NetworkManager.Singleton.OnClientConnectedCallback += OnConnected;
        //    NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnected;
        //    NetworkManager.Singleton.OnTransportFailure += OnFailed;

        //    //EventManager.onGetIsConnected += GetIsConnected;
        //    //EventManager.onConnectToNetwork += Connect;
        //    //EventManager.onLeaveRoom += ExitRoom;

        //    if (PlayerPrefs.HasKey("ip") && PlayerPrefs.HasKey("port"))
        //    {
        //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(isServer ? "0.0.0.0" : PlayerPrefs.GetString("ip"), ushort.Parse(PlayerPrefs.GetString("port")));
        //    }
        //    else
        //    {
        //        SceneManager.LoadScene("ConfigScreen");
        //    }

        //    Debug.LogWarning("ip address : " + NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address + " " + NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port);
        //    if (isServer)
        //    {
        //        yield return new WaitForSeconds(2f);
        //        Connect();
        //    }
        //    Debug.LogWarning("IsServer : " + isServer);
        //}

        private void OnClientConnected(ulong clientId)
        {
            //Debug.LogWarning("connected : " + clientId);
            //EventManager.onReady?.Invoke(NetworkManager.Singleton.ConnectedClients.Count);

            //if (NetworkManager.Singleton.ConnectedClients.Count == MAX_PLAYER)
            //{
            //    GameManager.Instance.PrepareGame();
            //}
        }

        //private void OnDestroy()
        //{
        //    if (NetworkManager.Singleton != null)
        //    {
        //        if (isServer)
        //        {
        //            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        //        }
        //        NetworkManager.Singleton.OnClientConnectedCallback -= OnConnected;
        //        NetworkManager.Singleton.OnClientDisconnectCallback -= OnDisconnected;
        //        NetworkManager.Singleton.ConnectionApprovalCallback -= OnCheckApprovalConnection;
        //        //NetworkManager.Singleton.OnTransportFailure -= OnFailed;
        //    }
        //    EventManager.onConnectToNetwork -= Connect;
        //    EventManager.onLeaveRoom -= ExitRoom;
        //}

        //private void Log(NetworkEvent eventType, ulong clientId, ArraySegment<byte> payload, float receiveTime)
        //{
        //    Debug.Log(eventType + " " + clientId);
        //}

        //private bool GetIsConnected()
        //{
        //    return NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer;
        //}

        //private void OnFailed()
        //{
        //    errorMessage = "Failed";
        //    Debug.LogWarning("transport failed");
        //}

        //public void Connect()
        //{
        //    Debug.Log("Connect");
        //    if (isServer)
        //    {
        //        if (NetworkManager.Singleton.StartServer())
        //        {
        //            errorMessage = "Server connected";
        //            EventManager.onNetworkConnected?.Invoke(NetworkManager.Singleton.LocalClientId);
        //            Debug.LogWarning("server started");
        //        }
        //        else
        //        {
        //            Debug.LogWarning("Failed");
        //        }
        //    }
        //    else
        //    {
        //        if (NetworkManager.Singleton.StartClient())
        //        {
        //            errorMessage = "client connected";
        //            EventManager.onNetworkConnected?.Invoke(NetworkManager.Singleton.LocalClientId);
        //            Debug.LogWarning("client started");
        //        }
        //        else
        //        {
        //            Debug.LogWarning("Failed " + NetworkManager.Singleton.DisconnectReason);
        //        }
        //    }
        //}


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

        //#region NetCode

        //private void OnCheckApprovalConnection(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse resp)
        //{
        //    resp.Approved = (NetworkManager.Singleton.ConnectedClientsIds.Count < 5);
        //    Debug.LogWarning("Check Approval " + resp.Reason);
        //}

        //public void OnServerStarted()
        //{
        //    Debug.Log("server started");
        //    //GameManager.Instance.ShowExitRoomButton();
        //    //FootballController.Instance.ApplyRole();
        //    GameManager.Instance.InitGame();
        //    GameManager.Instance.OnServerConnected();
        //}

        //public void OnConnected(ulong clientId)
        //{
        //    if (clientId == NetworkManager.Singleton.LocalClientId)
        //    {
        //        //GameManager.Instance.ShowExitRoomButton();
        //        OnDevicePaired();
        //        GameManager.Instance.OnClientConnected();
        //    }
        //    else
        //    {
        //        GameManager.Instance.OnPlayerConnected(clientId);
        //        OnClientConnected(clientId);
        //        Debug.LogWarning("OnConnected : " + clientId + "  " + NetworkManager.Singleton.ConnectedClients.Count);
        //    }
        //    //FootballController.Instance.ApplyRole();
        //}

        //public void OnDisconnected(ulong clientId)
        //{
        //    Debug.LogWarning("Disconnect : " + clientId + " " + NetworkManager.Singleton.IsServer + " " + NetworkManager.Singleton.DisconnectReason);
        //    if (clientId == NetworkManager.Singleton.LocalClientId)
        //    {
        //        //GameManager.Instance.ShowReconnectButton();
        //    }
        //    if(isServer)
        //    {
        //        EventManager.onDisconnected?.Invoke(clientId);
               
        //    }

        //}

        ////public void OnApplicationFocus(bool focus)
        ////{
        ////    if (NetworkManager.Singleton.IsServer)
        ////    {
        ////        Connect();
        ////    }
        ////}

        public void OnJoinedRoom()
        {
        }


        //#endregion

        private void OnDevicePaired()
        {
            //connectedDeviceCount += 1;
            //EventManager.onReady?.Invoke(connectedDeviceCount);
        }

        //private void OnGUI()
        //{
        //    GUIStyle style = new GUIStyle();
        //    style.fontSize = 50;
        //    style.normal.textColor = Color.red;

        //    GUI.Label(new Rect(40, 100, 300, 60), "IP : " + ((UnityTransport)transport).ConnectionData.Address.ToString() + ":" + ((UnityTransport)transport).ConnectionData.Port + " " + ((UnityTransport)transport).ConnectionData.ServerListenAddress.ToString(), style);
        //    GUI.Label(new Rect(40, 160, 300, 60), "msg : " + errorMessage, style);

        //}

        public void SendMove(Vector2 direction)
        {
            EventManager.onMoveInputChanged?.Invoke(direction);
        }

        public void SendStartJump()
        {
            EventManager.onJumpPressed?.Invoke();
        }

        public void SendStopJump()
        {
            EventManager.onJumpReleased?.Invoke();
        }
    }
}
