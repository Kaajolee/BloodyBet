using UnityEngine;

public class Card
{
    public Suit Suit { get; set; }

    public Rank Rank { get; set; }

    public Card(Rank rank)
    {
        Rank = rank;
    }

    public override string ToString()
    {
        return $"{Rank}";
    }
}

public enum Suit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades,
}

public enum Rank
{
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11, 
    Queen = 12,
    King = 13,
}