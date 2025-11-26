using System.Collections.Generic;
using UnityEngine;

public class RouletteLogic : MonoBehaviour
{
    public List<RouletteBet> playerBets = new List<RouletteBet>();

    public void BeginBetting()
    {
        playerBets.Clear();
    }

    public void AddBet(RouletteBet bet)
    {
        playerBets.Add(bet);
    }

    public int SpinWheel()
    {
        // 0-36 standard wheel
        int randomNumber = Random.Range(0, 37);
        return randomNumber;
    }

    public int CalculatePayout(int resultNumber)
    {
        int totalWin = 0;

        foreach (var bet in playerBets)
        {
            totalWin += bet.Evaluate(resultNumber);
        }

        return totalWin;
    }

    public void ResetTable()
    {
        playerBets.Clear();
    }
}
