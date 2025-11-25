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

        foreach (Card card in hand.GetCards())
        {
            int value = (int)Enum.Parse(typeof(Rank), card.Rank.ToString());

            if (value > 10)
            {
                sum += 10;
            }
            else if (value <= 10 && value != 1)
            {
                sum += value;
            }
            else if (value == 1) //ace
            {
                if (sum + value > 21)
                    sum += value;
                else
                    sum += 11;
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