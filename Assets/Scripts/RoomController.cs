using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomController : MonoBehaviour, IPointerClickHandler
{
    public Text nameText;
    public Text playerInformationText;

    public void OnPointerClick(PointerEventData eventData)
    {
        WebSocketManeger.JoinRoom(nameText.text);
    }
}