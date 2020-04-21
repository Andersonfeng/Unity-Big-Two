using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class PokerController : MonoBehaviour, IComparable<PokerController>
{
    public Poker poker;
    public bool select;

    public PokerController(int point, Color color)
    {
        this.poker.point = point;
        this.poker.color = color;
    }

    [Serializable]
    public class Poker : IComparable<Poker>
    {
        [Range(1, 15)] public int point;

        public Color color;

        public int CompareTo(Poker other)
        {
            return point.CompareTo(other);
        }
    }

    public enum Color
    {
        黑桃 = 4,
        红桃 = 3,
        梅花 = 2,
        方块 = 1
    }

    public enum CardType
    {
        单张 = 1,
        对子 = 2,
        干炒 = 3,
        蛇 = 4,
        同花 = 5,
        葫芦 = 6,
        金刚 = 7,
        同花顺 = 8
    }

    private void Start()
    {
        var button = gameObject.AddComponent<Button>();
        button.onClick.AddListener(onClick);
    }

    public void onClick()
    {
        select = !select;
        if (select)
            transform.position += Vector3.up * 25;
        else
        {
            transform.position += Vector3.down * 25;
        }
    }

    public int CompareTo(PokerController other)
    {
        if (poker.point > other.poker.point)
            return 1;
        if (poker.point.Equals(other.poker.point) && poker.color > other.poker.color)
            return 1;
        return -1;
    }
}