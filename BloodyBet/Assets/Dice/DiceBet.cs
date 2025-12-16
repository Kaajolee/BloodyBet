using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

public class DiceBet : MonoBehaviour {
    [Header("XR Dice Buttons (1–6)")]
    [SerializeField] private XRSimpleInteractable sideOne;
    [SerializeField] private XRSimpleInteractable sideTwo;
    [SerializeField] private XRSimpleInteractable sideThree;
    [SerializeField] private XRSimpleInteractable sideFour;
    [SerializeField] private XRSimpleInteractable sideFive;
    [SerializeField] private XRSimpleInteractable sideSix;

    [Header("Managers")]
    [SerializeField] private DiceRoller diceRoller;
    [SerializeField] private CurrencyManager currencyManager;

    private int chosenSide = -1;
    private int placedBet = 0;
    private bool waitingForResult = false;

    void Start() {
        diceRoller.ResetValue();
    }

    void Awake() {
        sideOne.selectEntered.AddListener(_ => TryPlaceBet(1));
        sideTwo.selectEntered.AddListener(_ => TryPlaceBet(2));
        sideThree.selectEntered.AddListener(_ => TryPlaceBet(3));
        sideFour.selectEntered.AddListener(_ => TryPlaceBet(4));
        sideFive.selectEntered.AddListener(_ => TryPlaceBet(5));
        sideSix.selectEntered.AddListener(_ => TryPlaceBet(6));
    }

    private void TryPlaceBet(int side) {
        if (waitingForResult)
            return;

        placedBet = currencyManager.currentBet;

        if (placedBet <= 0 || currencyManager.Balance < placedBet) {
            Debug.Log("NOT ENOUGH MONEY TO PLACE BET");
            return;
        }

        currencyManager.RemoveMoney(placedBet);

        diceRoller.ResetValue();

        chosenSide = side;
        waitingForResult = true;

        Debug.Log($"Player bet {placedBet} on {chosenSide}");
        Debug.Log("ROLL THE DICE");

        StartCoroutine(WaitForDiceResult());
    }

    private IEnumerator WaitForDiceResult() {
        yield return new WaitUntil(() => diceRoller.GetCurrentValue() != 0);

        int result = diceRoller.GetCurrentValue();

        if (result == chosenSide) {
            int winAmount = placedBet * 2;
            currencyManager.AddMoney(winAmount);
            Debug.Log($"WIN! Rolled {result}, won {winAmount}");
        } else {
            Debug.Log($"LOSE! Rolled {result}, lost {placedBet}");
        }

        chosenSide = -1;
        placedBet = 0;
        waitingForResult = false;
    }
}
