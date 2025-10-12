using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class BlackjackLogic : MonoBehaviour
{
    public event Action OnPlayerBust;
    public event Action OnDealerBust;
    public event Action OnRoundReset;


    public List<BlackjackHand> DealerHands = new List<BlackjackHand>();

    public List<BlackjackHand> PlayerHands = new List<BlackjackHand>();

    private List<Card> deck = new List<Card>();     

    private static System.Random rng = new System.Random();

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void StartGame()
    {
        GenerateDeck();
        ShuffleDeck();

        PlayerHands.Clear();
        DealerHands.Clear();

        PlayerHands.Add(new BlackjackHand());
        DealerHands.Add(new BlackjackHand());
    }

    public void GenerateDeck()
    {
        var suits = Enum.GetValues(typeof(Suit));
        var ranks = Enum.GetValues(typeof(Rank));

        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                Card newCard = new Card((Suit)suit, (Rank)rank);

                deck.Add(newCard);
            }
        }
    }

    public void ShuffleDeck()
    {
        deck = deck.OrderBy(_ => rng.Next()).ToList();
    }

    public void DealCard(BlackjackHand hand)
    {
        Card card = deck.FirstOrDefault();
        
        hand.Insert(card);
        deck.Remove(card);

        int total = EvaluateHand(hand);

        if (hand == PlayerHands.First() && total > 21)
        {
            Debug.Log("Player busts!");
            OnPlayerBust?.Invoke();
        }
        else if (hand == DealerHands.First() && total > 21)
        {
            Debug.Log("Dealer busts!");
            OnDealerBust?.Invoke();
        }
    }

    public int EvaluateHand(BlackjackHand hand)
    {
        int sum = 0;

        foreach (Card card in hand.GetCards())
        {
            int value = (int)Enum.Parse(typeof(Rank), card.Rank.ToString());

            if (value > 10)
            {
                sum += 10;
            }
            else if (value <= 10)
            {
                sum += value;
            }
        }

        return sum;
    }
}
