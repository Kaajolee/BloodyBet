using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class BlackjackHand
{
    private List<Card> Cards = new List<Card>();

    public BlackjackHand()
    { }

    public List<Card> GetCards()
    {
        return Cards;
    }

    public void Insert(Card card)
    {
        Cards.Add(card);
    }

    public void Remove(int index)
    {
        Cards.RemoveAt(index);
    }

    public void Clear()
    {
        Cards.Clear();
    }

    public Card GetLastCard() => Cards.Last();
}
