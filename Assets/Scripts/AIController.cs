using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIController : PlayerAble
{
    private List<GameObject> _handCard = new List<GameObject>();
    private bool _select;

    public override List<GameObject> SetCard(List<GameObject> cardList)
    {
        while (_handCard.Count < 13)
        {
            var index = Random.Range(0, cardList.Count - 1);
            var cardPrefab = cardList[index];

            var position =
                new Vector2(
                    handZone.transform.position.x +
                    _handCard.Count * cardPrefab.GetComponent<RectTransform>().rect.width,
                    handZone.transform.position.y);
            var card = Instantiate(cardPrefab, position, Quaternion.identity);
            card.transform.SetParent(handZone.transform);
            card.GetComponent<RectTransform>().rotation = new Quaternion();
            card.GetComponent<PokerController>().enabled = false;
            var pokerController = card.GetComponent<PokerController>();
            pokerController.enabled = false;
            if (pokerController.poker.point.Equals(3) && pokerController.poker.color == PokerController.Color.方块)
            {
                SetFirstTurn();
                SetTurn();
            }

            _handCard.Add(card);
            cardList.RemoveAt(index);
        }

        Invoke("SoftCard", 0.01f);
        return cardList;
    }

    public override void RefreshCard(List<GameObject> cardList)
    {
        _handCard = cardList;
    }

    public override List<GameObject> GetHandCard()
    {
        return _handCard;
    }

    private void Update()
    {
        if (GetFirstTurn())
        {
            PlayFirstTurn();
            return;
        }

        if (GetTurn())
        {
            SelectCard();
            Invoke("PlayCard", 1f);
        }
    }

    private void PlayFirstTurn()
    {
        foreach (var o in _handCard)
        {
            var pokerController = o.GetComponent<PokerController>();
            if (pokerController.poker.point.Equals(3) && pokerController.poker.color == PokerController.Color.方块)
                pokerController.@select = true;
        }

        GameManager.PlayCard(GetIndex(),_handCard, true);
        DropFirstTurn();
    }

    private void SelectCard()
    {
        if (_select)
            return;
        _select = true;
        var lastPlay = GameManager.GetLastPlay();
        switch (lastPlay.Count)
        {
            case 0:
                _handCard[0].GetComponent<PokerController>().@select = true;
                break;
            case 1:
                SelectOne(lastPlay);
                break;
            case 2:
                Pass();
                break;
            case 3:
                Pass();
                break;
            case 5:
                Pass();
                break;
        }
    }

    /**
     * 选一张牌
     * 比上一个玩家打出的牌大
     */
    private void SelectOne(List<GameObject> lastPlay)
    {
        var lastPokerController = lastPlay[0].GetComponent<PokerController>();
        foreach (var card in _handCard)
        {
            var pokerController = card.GetComponent<PokerController>();
            if (pokerController.poker.point > lastPokerController.poker.point)
            {
                pokerController.@select = true;
                return;
            }

            if (pokerController.poker.point.Equals(lastPokerController.poker.point)
                && pokerController.poker.color > lastPokerController.poker.color)
            {
                pokerController.@select = true;
                return;
            }
        }

        Pass();
    }

    public void PlayCard()
    {
        _select = false;
        GameManager.PlayCard(GetIndex(),_handCard);
    }

    public void SoftCard()
    {
        for (var i = 0; i < _handCard.Count; i++)
        {
            for (var j = 0; j < _handCard.Count - 1 - i; j++)
            {
                var pokerPoint = _handCard[j].GetComponent<PokerController>().poker.point;
                var nextpokerPokerPoint = _handCard[j + 1].GetComponent<PokerController>().poker.point;
                if (pokerPoint > nextpokerPokerPoint)
                {
                    var temp = _handCard[j];
                    _handCard[j] = _handCard[j + 1];
                    _handCard[j + 1] = temp;
                    SwapTransformPosition(_handCard[j].transform, _handCard[j + 1].transform);
                }
            }
        }

        handZone.transform.DetachChildren();

        foreach (var card in _handCard)
        {
            card.transform.SetParent(handZone.transform);
        }
    }

    private void SwapTransformPosition(Transform t1, Transform t2)
    {
        var temp = t1.position;
        t1.position = t2.position;
        t2.position = temp;
    }

    /**
     * 判断牌里的对子
     */
    private void CheckPair()
    {
        //todo 选最小的对子来出
        var handCardList = _handCard.ConvertAll(x => x.GetComponent<PokerController>());
        var selectedCard = handCardList.GroupBy(x => x.poker.point)
            .Select(card => new {key = card.Key, total = card.Count()})
            .ToDictionary(x => x.key, x => x.total);

        var i = selectedCard[2];
    }
}