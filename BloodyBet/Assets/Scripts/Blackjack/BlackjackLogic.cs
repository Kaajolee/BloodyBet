using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class BlackjackLogic : MonoBehaviour
{
    [SerializeField]
    private List<Card> DealerHand = new List<Card>();

    [SerializeField]
    private List<Card> PlayerHand = new List<Card>();

    private List<Card> deck = new List<Card>();     

    private static System.Random rng = new System.Random();

    void Start()
    {
        GenerateDeck();

        //foreach (Card card in deck) 
        //{ 
        //    Debug.Log(card.ToString());
        //}
        
        ShuffleDeck();

        DealCard(DealerHand);
        DealCard(PlayerHand);
        DealCard(DealerHand);
        DealCard(PlayerHand);

        foreach (Card card in DealerHand)
        {
            Debug.Log("Dealer: " + card.ToString());
        }

        foreach (Card card in PlayerHand)
        {
            Debug.Log("Player: " + card.ToString());
        }

        Debug.Log("Player hand value: " + EvaluateHand(PlayerHand));

    }

    void Update()
    {
        
    }

    private void GenerateDeck()
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

    public void DealCard(List<Card> hand)
    {
        Card card = deck.FirstOrDefault();
        
        hand.Add(card);
        deck.Remove(card);
    }

    public int EvaluateHand(List<Card> hand)
    {
        int sum = 0;

        foreach (Card card in hand)
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
