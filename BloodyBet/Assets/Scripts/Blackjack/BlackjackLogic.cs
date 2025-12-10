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

    private bool playerBusted = false;
    private bool dealerBusted = false;

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

        playerBusted = false;
        dealerBusted = false;

        PlayerHands.Add(new BlackjackHand());
        DealerHands.Add(new BlackjackHand());
    }

    public void GenerateDeck()
    {
        //var suits = Enum.GetValues(typeof(Suit));
        var ranks = Enum.GetValues(typeof(Rank));

        //foreach (var suit in suits)
        //{
            foreach (var rank in ranks)
            {
                Card newCard = new Card((Rank)rank);

                deck.Add(newCard);
            }
       // }
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
            playerBusted = true;
            OnPlayerBust?.Invoke();
        }
        else if (hand == DealerHands.First() && total > 21)
        {
            Debug.Log("Dealer busts!");
            dealerBusted = true;
            OnDealerBust?.Invoke();
        }
    }

    public int EvaluateHand(BlackjackHand hand)
    {
        int sum = 0;
        int aceCount = 0;

        // 1. Sum up all non-Ace cards and count the Aces
        foreach (Card card in hand.GetCards())
        {
            // Get the numeric value (assuming Rank 2-10, J/Q/K=10, Ace=1)
            int cardValue = (int)Enum.Parse(typeof(Rank), card.Rank.ToString());

            if (cardValue >= 10) // J, Q, K
            {
                sum += 10;
            }
            else if (cardValue > 1) // 2 through 10 (or equivalent enum value)
            {
                sum += cardValue;
            }
            else if (cardValue == 1) // Ace
            {
                aceCount++;
            }
        }

        // 2. Add Aces, prioritizing the 11 value
        for (int i = 0; i < aceCount; i++)
        {
            // Try to count the Ace as 11 first (soft hand)
            if (sum + 11 <= 21)
            {
                sum += 11;
            }
            else
            {
                // If 11 would bust the hand, count it as 1 instead (hard hand)
                sum += 1;
            }
        }

        return sum;
    }

    public RoundInfo EvaluateRound()
    {
        var playerTotal = EvaluateHand(PlayerHands.First());
        var dealerTotal = EvaluateHand(DealerHands.First());

        if (playerBusted && dealerBusted)
        {
            return new RoundInfo(RoundOutcome.Push, playerTotal, dealerTotal);
        }
        else if (playerBusted)
        {
            return new RoundInfo(RoundOutcome.DealerWin, playerTotal, dealerTotal);
        }
        else if (dealerBusted)
        {
            return new RoundInfo(RoundOutcome.PlayerWin, playerTotal, dealerTotal);
        }
        else if (playerTotal > dealerTotal)
        {
            return new RoundInfo(RoundOutcome.PlayerWin, playerTotal, dealerTotal);
        }
        else if (playerTotal < dealerTotal)
        {
            return new RoundInfo(RoundOutcome.DealerWin, playerTotal, dealerTotal);
        }
        else // playerTotal == dealerTotal
        {
            return new RoundInfo(RoundOutcome.Push, playerTotal, dealerTotal);
        }
    }
}

public struct RoundInfo
{
    public RoundInfo(RoundOutcome outcome, int playerScore, int dealerScore)
    {
        Outcome = outcome;
        PlayerScore = playerScore;
        DealerScore = dealerScore;
    }

    public RoundOutcome Outcome { get; private set; }
    public int PlayerScore {  get; private set; }
    public int DealerScore { get; private set; }
}

public enum RoundOutcome
{
    PlayerWin,
    DealerWin,
    Push,
}