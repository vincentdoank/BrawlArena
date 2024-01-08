using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Action onConnectToNetwork;
    public static Action<ulong> onDisconnected;
    public static Action onCreateRoom;
    public static Action<string> onJoinRoom;
    public static Action onLeaveRoom;
    public static Func<bool> onGetIsConnected;

    public static Action<int> onReady;

    public static Action<ulong> onNetworkConnected;

    #region NETWORKING

    public static Action onGameStarted;
    public static Action<Vector2> onMoveInputChanged;
    public static Action onJumpPressed;
    public static Action onJumpReleased;
    //public static Action<ulong, ulong> onOtherPlayerDisconnected;

    public static Action<ulong> onPlayerDead;
    public static Action onPlayerReady;
    public static Action onGameEnded;

    #endregion

    public static Action<string> sendNetworkMessage;
    public static Action<float> onProgressLoadingUpdated;

}
