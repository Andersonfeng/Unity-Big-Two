using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    public GameObject roomPrefab;
    public GameObject roomListPanel;
    public InputField usernameInputField;
    public InputField roomNameInputField;
    public InputField serverAddressInputField;
    private string _roomName;
    private static RoomManager _instance;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        usernameInputField.onEndEdit.AddListener(delegate(string username) { WebSocketManeger.SetUsername(username); });
        roomNameInputField.onEndEdit.AddListener(delegate(string roomName) { _roomName = roomName; });
        serverAddressInputField.onEndEdit.AddListener(delegate(string serverAddress)
        {
            WebSocketManeger.SetServerAddress(serverAddress);
        });
    }

    public void CreateRoom()
    {
        WebSocketManeger.CreateRoom(_roomName);
    }

    /**
     * 刷新房间
     */
    public static void RefreshRoomList(List<WebSocketManeger.Room> roomList)
    {
        ClearRoom();
        foreach (var room in roomList)
        {
            var roomObject = Instantiate(_instance.roomPrefab, _instance.roomListPanel.transform.position,
                Quaternion.identity);
            var roomController = roomObject.GetComponent<RoomController>();
            roomController.nameText.text = room.name;
            string playerInformation = "(" + room.playerCount + ")" + String.Join(",", room.playerNameList.ToArray());
            roomController.playerInformationText.text = playerInformation;
            roomObject.transform.SetParent(_instance.roomListPanel.transform);
        }
    }

    /**
     * 删除所有子节点
     */
    private static void ClearRoom()
    {
        // var roomListTransForm = _instance.roomListPanel.transform;

        List<Transform> childList = new List<Transform>();
        foreach (Transform child in _instance.roomListPanel.transform)
        {
            childList.Add(child);
        }

        childList.ForEach(child => Destroy(child.gameObject));
        // for (var i = roomListTransForm.childCount - 1; i > 0; i--)
        // {
        //     Destroy(roomListTransForm.GetChild(i).gameObject);
        // }
    }

    public void JoinRoom(BaseEventData data)
    {
        var roomName = data.selectedObject.transform.GetChild(0).GetComponent<Text>();
        Debug.Log("roomName:" + roomName);
    }
}