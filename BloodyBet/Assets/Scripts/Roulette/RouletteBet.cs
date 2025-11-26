using UnityEngine;

public class RouletteBet : MonoBehaviour
{
    public BetType type;
    public int number;
    public int amount;

    public int Evaluate(int result)
    {
        switch (type)
        {
            case BetType.SingleNumber:
                return result == number ? amount * 35 : 0;

            case BetType.Even:
                return (result != 0 && result % 2 == 0) ? amount * 2 : 0;

            case BetType.Odd:
                return (result != 0 && result % 2 == 1) ? amount * 2 : 0;
        }

        return 0;
    }
}

public enum BetType
{
    SingleNumber,
    Even,
    Odd
}
