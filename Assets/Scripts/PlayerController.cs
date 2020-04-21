using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerAble
{
    private List<GameObject> _handCard = new List<GameObject>();

    public override List<GameObject> SetCard(List<GameObject> cardList)
    {
        _handCard.Clear();
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

            _handCard.Add(card);
            cardList.RemoveAt(index);
        }

        if (HaveCube3())
        {
            SetFirstTurn();
            SetTurn();
        }

        Invoke("SoftCard", 0.01f);
        return cardList;
    }


    /**
     * 查看是否有方块3
     */
    private bool HaveCube3()
    {
        foreach (var cardObject in _handCard)
        {
            var poker = cardObject.GetComponent<PokerController>();
            if (poker.poker.point.Equals(3)
                && poker.poker.color == PokerController.Color.方块)
                return true;
        }

        return false;
    }

    public override void RefreshCard(List<GameObject> cardList)
    {
        foreach (Transform child in handZone.transform)
        {
            Destroy(child.gameObject);
        }

        _handCard = cardList;
        _handCard.ForEach(card => card.transform.SetParent(handZone.transform));
        SoftCard();

        //todo 测试
        // if (cardList.Count.Equals(13) && HaveCube3())
        {
            //通知服务器我第一轮有方块3 先出牌
            SetFirstTurn();
            SetTurn();
        }
    }

    public override List<GameObject> GetHandCard()
    {
        //清空原来手牌
        //放入新手牌
        return _handCard;
    }

    public void PlayCard()
    {
        if (GetFirstTurn())
        {
            GameManager.PlayCard(GetIndex(),_handCard, GetFirstTurn());
            DropFirstTurn();
        }

        if (GetTurn())
            GameManager.PlayCard(GetIndex(),_handCard);
    }


    /**
     * 卡牌排序
     * 排序后才能点击卡牌
     */
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
            card.GetComponent<PokerController>().enabled = true;
        }
    }


    private void SwapTransformPosition(Transform t1, Transform t2)
    {
        var temp = t1.position;
        t1.position = t2.position;
        t2.position = temp;
    }

 
}