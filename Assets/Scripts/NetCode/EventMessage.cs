using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using System;

namespace WTI.NetCode
{
    [Serializable]
    public class PositionListData
    {
        public List<Vector3> data;
    }

    [Serializable]
    public class Parameter
    {
        public object value;
        public Type type;
    }

    [Serializable]
    public class EventMessageData
    {
        public string methodName;
        public string[] parameters;
        public Type[] parameterTypes;
    }

    public class EventMessage : NetworkBehaviour
    {
        private const byte GAME_START_EVENT_CODE = 1;
        private const byte INPUT_DIRECTION_EVENT_CODE = 2;
        private const byte INPUT_START_JUMP_EVENT_CODE = 3;
        private const byte INPUT_STOP_JUMP_EVENT_CODE = 4;
        private const byte PLAYER_DEAD_EVENT_CODE = 5;
        private const byte PLAYER_READY_EVENT_CODE = 6;
        private const byte GAME_END_EVENT_CODE = 7;

        private const byte DISCONNECT_EVENT_CODE = 00;

        public enum TargetMessage
        {
            All = 0,
            AllClient = 1,
            Client = 2,
            Server = 3
        }

        private void Start()
        {
            EventManager.onGameStarted += SendGameStart;
            EventManager.onMoveInputChanged += SendUpdateMove;
            EventManager.onJumpPressed += SendStartJump;
            EventManager.onJumpReleased += SendStopJump;
            EventManager.onPlayerDead += SendPlayerDead;
            EventManager.onPlayerReady += SendPlayerReady;
            EventManager.onGameEnded += SendGameEnd;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        //private void OnNetworkConnected()
        //{
        //    NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler(messageName, ReceiveMessage);
        //    Debug.Log("OnNetworkSpawn");
        //}

        public override void OnNetworkSpawn()
        {
            //NetworkManager.CustomMessagingManager.OnUnnamedMessage += ReceiveMessage;
            Debug.Log("OnNetworkSpawn");
        }

        public override void OnNetworkDespawn()
        {
            //NetworkManager.CustomMessagingManager.OnUnnamedMessage -= ReceiveMessage;
        }

        public void SendUnnamedMessage(string dataToSend)
        {
            Debug.Log("data to send : " + dataToSend);
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            // Tip: Placing the writer within a using scope assures it will
            // be disposed upon leaving the using scope
            using (writer)
            {
                // Write our message type
                writer.WriteValueSafe(MessageType());

                // Write our string message
                writer.WriteValueSafe(dataToSend);
                if (IsServer)
                {
                    // This is a server-only method that will broadcast the unnamed message.
                    // Caution: Invoking this method on a client will throw an exception!
                    customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    // This method can be used by a client or server (client to server or server to client)
                    customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                }
            }
        }

        //private void SendOtherPlayerDisconnectedStat(ulong senderClientId, ulong clientId)
        //{
        //    NetworkController.Instance.errorMessage = "SendOtherPlayerDisconnectedStat";
        //    Debug.Log("SendOtherPlayerDisconnectedStat");
        //    var writer = new FastBufferWriter(1100, Allocator.Temp);
        //    var customMessagingManager = NetworkManager.CustomMessagingManager;
        //    using (writer)
        //    {

        //        // Write our message type
        //        writer.WriteValueSafe(DISCONNECT_EVENT_CODE);
        //        writer.WriteValueSafe(clientId);

        //        if (IsServer)
        //        {
        //            customMessagingManager.SendUnnamedMessageToAll(writer);
        //        }
        //        else
        //        {
        //            if (NetworkManager.IsConnectedClient)
        //            {
        //                // This method can be used by a client or server (client to server or server to client)
        //                customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
        //            }
        //        }
        //    }
        //    Debug.Log("SendOtherPlayerDisconnectedStat Succeed");
        //}

        protected virtual byte MessageType()
        {
            return 0;
        }

        private void ReceiveMessage(ulong clientId, FastBufferReader reader)
        {
            // Read the message type value that is written first when we send
            // this unnamed message.
            reader.ReadValueSafe(out byte messageType);
            // Example purposes only, you might handle this in a more optimal way
            //Debug.Log("message type : " + messageType.ToString());
            switch (messageType)
            {
                //case 0:
                //    OnReceivedUnnamedMessage(clientId, reader);
                //    break;
                case GAME_START_EVENT_CODE:
                    OnReceivedGameStartEventMessage(reader);
                    break;
                case INPUT_DIRECTION_EVENT_CODE:
                    OnReceivedInputDirectionEventMessage(clientId, reader);
                    break;
                case INPUT_START_JUMP_EVENT_CODE:
                    OnReceivedInputStartJumpEventMessage(clientId, reader);
                    break;
                case INPUT_STOP_JUMP_EVENT_CODE:
                    OnReceivedInputStopJumpEventMessage(clientId, reader);
                    break;
                case PLAYER_DEAD_EVENT_CODE:
                    OnReceivedPlayerDeadEventMessage(clientId, reader);
                    break;
                case PLAYER_READY_EVENT_CODE:
                    OnReceivedPlayerReadyEventMessage(clientId, reader);
                    break;
                case GAME_END_EVENT_CODE:
                    OnReceivedGameEndEventMessage(reader);
                    break;
            }
        }

        protected void OnReceivedUnnamedMessage(ulong clientId, FastBufferReader reader)
        {
            var stringMessage = string.Empty;
            reader.ReadValueSafe(out stringMessage);
            if (IsServer)
            {
                Debug.Log($"Server received unnamed message of type ({MessageType()}) from client " +
                    $"({clientId}) that contained the string: \"{stringMessage}\"");

                // As an example, we could also broadcast the client message to everyone
                SendUnnamedMessage($"Newly connected client sent this greeting: \"{stringMessage}\"");
            }
            else
            {
                Debug.Log(stringMessage);
            }
        }

        protected void OnReceivedGameStartEventMessage(FastBufferReader reader)
        {
            Debug.Log("OnReceivedGameStart");
            if (IsServer)
            {
                SendGameStart();
            }
            OnGameStarted();
        }

        protected void OnReceivedInputDirectionEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("received OnReceivedInputDirectionEventMessage");

            reader.ReadValueSafe(out Vector2 direction);
            OnPositionUpdated(clientId, direction);
        }

        protected void OnReceivedInputStartJumpEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("received OnReceivedInputStartJumpEventMessage");
            OnJumpStarted(clientId);
        }

        protected void OnReceivedInputStopJumpEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("received OnReceivedInputStopJumpEventMessage");
            OnJumpStopped(clientId);
        }

        protected void OnReceivedPlayerDeadEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("received OnReceivedPlayerDeadEventMessage");
            OnPlayerDead();
        }

        protected void OnReceivedPlayerReadyEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("received OnReceivedPlayerReadyEventMessage");
            OnPlayerReady(clientId);
        }

        protected void OnReceivedGameEndEventMessage(FastBufferReader reader)
        {
            Debug.Log("OnReceivedGameEndEventMessage");
            if (IsServer)
            {
                SendGameEnd();
            }
            OnGameEnded();
        }

        private void OnGameStarted()
        {
            GameManager.Instance.HidePreGameUI();
        }

        private void OnPositionUpdated(ulong clientId, Vector2 direction)
        {
            GameManager.Instance.MoveCharacter(clientId, direction);
        }

        private void OnJumpStarted(ulong clientId)
        {
            GameManager.Instance.StartJumpCharacter(clientId);
        }

        private void OnJumpStopped(ulong clientId)
        {
            GameManager.Instance.StopJumpCharacter(clientId);
        }

        private void OnPlayerDead()
        {
            GameManager.Instance.PlayerDead();
        }

        private void OnPlayerReady(ulong clientId)
        {
            GameManager.Instance.PlayerReady(clientId);
        }

        private void OnGameEnded()
        {
            GameManager.Instance.ShowRestartButton();
        }

        private void SendGameStart()
        {
            Debug.LogWarning("send game start");
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {
                // Write our message type
                writer.WriteValueSafe(GAME_START_EVENT_CODE);

                if (IsServer)
                {
                    customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendUpdateMove(Vector2 direction)
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {
                // Write our message type
                writer.WriteValueSafe(INPUT_DIRECTION_EVENT_CODE);
                writer.WriteValueSafe(direction);

                if (IsServer)
                {
                    //customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendStartJump()
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {
                // Write our message type
                writer.WriteValueSafe(INPUT_START_JUMP_EVENT_CODE);

                if (IsServer)
                {
                    //customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendStopJump()
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {
                // Write our message type
                writer.WriteValueSafe(INPUT_STOP_JUMP_EVENT_CODE);

                if (IsServer)
                {
                    //customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendPlayerDead(ulong clientId)
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {
                // Write our message type
                writer.WriteValueSafe(PLAYER_DEAD_EVENT_CODE);

                if (IsServer)
                {
                    customMessagingManager.SendUnnamedMessage(clientId, writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendPlayerReady()
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {
                // Write our message type
                writer.WriteValueSafe(PLAYER_READY_EVENT_CODE);

                if (IsServer)
                {
                    //customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendGameEnd()
        {
            Debug.LogWarning("send game end");
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {
                // Write our message type
                writer.WriteValueSafe(GAME_END_EVENT_CODE);

                if (IsServer)
                {
                    customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }
    }
}
