using System;
using System.Collections.Generic;
using BestHTTP.WebSocket;
using LitJson;
using UnityEngine;
using Object = System.Object;

public class WebSocketManeger : MonoBehaviour
{
    private WebSocket _webSocket;
    private static WebSocketManeger _instance;
    private string _username = "no name";
    private bool _modify;
    private string _serverAddress;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        _serverAddress = "ws://andersonfeng.asuscomm.com:8081/WebSocketServer/webSocket/room/";
    }

    [Serializable]
    public class CommonResponse<T>
    {
        public int code;

        public string message;

        // public List<Room> data;
        public T data;
    }

    [Serializable]
    public class Room
    {
        public string name;
        public int roomNumber;
        public int playerCount;
        public List<string> playerNameList;
    }

    [Serializable]
    public class Player
    {
        public int index;
        public String name;
        public List<PokerController.Poker> poker;
    }

    public static void SetUsername(string username)
    {
        if (!username.Equals(_instance._username))
        {
            _instance._username = username;
            _instance._modify = true;
        }

        if (_instance._webSocket != null && _instance._webSocket.IsOpen)
            _instance._webSocket.Close();
    }

    public void ConnectByBestHttp()
    {
        //如果已经有连接 而且用户名没变 不做任何连接操作
        if (_webSocket != null && _webSocket.IsOpen && !_instance._modify)
            return;

        _instance._modify = false;
        _webSocket = new WebSocket(new Uri(_serverAddress + _username));
        _webSocket.OnOpen += OnWebSocketOpen;
        _webSocket.OnMessage += OnMessage;
        _webSocket.OnError += OnError;
        _webSocket.OnClosed += OnWebSocketClosed;

        _webSocket.Open();
    }

    private void OnWebSocketClosed(WebSocket websocket, ushort code, string message)
    {
        GameManager.ShowText("连接中断了");
        Debug.Log("WebSocket Closed!");
    }

    private void OnError(WebSocket websocket, Exception ex)
    {
        Debug.Log("OnError" + ex);
    }

    public void Close()
    {
        _webSocket.Close();
    }

    private void OnMessage(WebSocket websocket, string message)
    {
        Debug.Log("OnMessage" + message);
        var commonResponse = JsonUtility.FromJson<CommonResponse<string>>(message);
        switch (commonResponse.code)
        {
            case 200:
            case 201:
                //roomList
                var roomList = JsonUtility.FromJson<CommonResponse<List<Room>>>(message).data;
                if (roomList != null && roomList.Count > 0)
                    RoomManager.RefreshRoomList(roomList);
                break;
            case 202:
                //room
                var room = JsonUtility.FromJson<CommonResponse<Room>>(message).data;
                GameManager.RefreshRoom(room, room.playerNameList.FindLastIndex(name => name.Equals(_username)));
                break;
            case 203:
                var handcardList = JsonMapper.ToObject<CommonResponse<List<PokerController.Poker>>>(message).data;
                GameManager.SetHandCard(handcardList);
                break;
            case 204:
                //其他玩家出牌
                var player = JsonMapper.ToObject<CommonResponse<Player>>(message).data;
                GameManager.SetLastPlay(player.index, player.poker);
                break;
            case 205:
                var turn = JsonMapper.ToObject<CommonResponse<int>>(message).data;
                GameManager.SetTurn(turn);
                break;
            case 206:
                GameManager.CleanDropZone();
                break;
            case 207: //有玩家PASS
                var passIndex = JsonMapper.ToObject<CommonResponse<int>>(message).data;
                GameManager.SetPass(passIndex);
                break;
            case 208: //更新房间玩家状态
                var playerList = JsonMapper.ToObject<CommonResponse<List<Player>>>(message).data;
                GameManager.RefreshPlayer(playerList);
                break;
            case 209: //玩家胜利
                var winMessage = JsonMapper.ToObject<CommonResponse<string>>(message).data;
                GameManager.AnnounceWin(winMessage);
                break;
        }
    }

    private void OnWebSocketOpen(WebSocket webSocket)
    {
        Debug.Log("WebSocket Open!");
        SendMessage("LIST");
    }

    private static void SendMessage(string message)
    {
        Debug.Log("sendMessage:" + message);
        _instance._webSocket.Send(message);
    }

    public static void JoinRoom(string roomName)
    {
        Debug.Log("准备加入房间:" + roomName);
        SendMessage("JOIN:" + roomName);
        GameManager.ClosePanel();
    }

    public static void CreateRoom(string roomName)
    {
        SendMessage("CREATE:" + roomName);
    }

    public static void SetHandCard(int index, string message, string roomName)
    {
        SendMessage("SET_HANDCARD:" + roomName + ":" + index + ":" + message);
    }

    public static void StartGame(string roomName)
    {
        SendMessage("START_GAME" + ":" + roomName);
    }

    public static void PlayCard(int index, string message, string roomName)
    {
        SendMessage("PLAY_CARD" + ":" + roomName + ":" + index + ":" + message);
    }

    public static void Pass(int index, string roomName)
    {
        SendMessage("PASS" + ":" + roomName + ":" + index);
    }

    public static void CleanDropZone(string roomName)
    {
        SendMessage("CLEAN_DROP_ZONE" + ":" + roomName);
    }

    public static void Win(string roomName, string username)
    {
        SendMessage("WIN" + ":" + roomName + ":" + username);
    }

    public static void SetServerAddress(string serverAddress)
    {
        _instance._serverAddress = serverAddress;
    }
}