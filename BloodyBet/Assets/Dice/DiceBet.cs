using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;
using TMPro;

public class DiceBet : MonoBehaviour {
    [Header("XR Dice Buttons (1–6)")]
    [SerializeField] private XRSimpleInteractable sideOne;
    [SerializeField] private XRSimpleInteractable sideTwo;
    [SerializeField] private XRSimpleInteractable sideThree;
    [SerializeField] private XRSimpleInteractable sideFour;
    [SerializeField] private XRSimpleInteractable sideFive;
    [SerializeField] private XRSimpleInteractable sideSix;

    [Header("XR Dice Buttons (>1 – >6)")]
    [SerializeField] private XRSimpleInteractable greaterThanOne;
    [SerializeField] private XRSimpleInteractable greaterThanTwo;
    [SerializeField] private XRSimpleInteractable greaterThanThree;
    [SerializeField] private XRSimpleInteractable greaterThanFour;
    [SerializeField] private XRSimpleInteractable greaterThanFive;
    [SerializeField] private XRSimpleInteractable greaterThanSix;

    [Header("Managers")]
    [SerializeField] private DiceRoller diceRoller;
    [SerializeField] private CurrencyManager currencyManager;

    [Header("UI")]
    [SerializeField] private TextMeshPro resultText;

    private int chosenSide = -1;
    private int placedBet = 0;
    private bool waitingForResult = false;

    // 🔹 ar tai „greater than“ statymas
    private bool isGreaterThanBet = false;

    void Start() {
        diceRoller.ResetValue();
    }

    void Awake() {
        sideOne.selectEntered.AddListener(_ => TryPlaceBetExact(1));
        sideTwo.selectEntered.AddListener(_ => TryPlaceBetExact(2));
        sideThree.selectEntered.AddListener(_ => TryPlaceBetExact(3));
        sideFour.selectEntered.AddListener(_ => TryPlaceBetExact(4));
        sideFive.selectEntered.AddListener(_ => TryPlaceBetExact(5));
        sideSix.selectEntered.AddListener(_ => TryPlaceBetExact(6));

        greaterThanOne.selectEntered.AddListener(_ => TryPlaceBetGreaterThan(1));
        greaterThanTwo.selectEntered.AddListener(_ => TryPlaceBetGreaterThan(2));
        greaterThanThree.selectEntered.AddListener(_ => TryPlaceBetGreaterThan(3));
        greaterThanFour.selectEntered.AddListener(_ => TryPlaceBetGreaterThan(4));
        greaterThanFive.selectEntered.AddListener(_ => TryPlaceBetGreaterThan(5));
        greaterThanSix.selectEntered.AddListener(_ => TryPlaceBetGreaterThan(6));
    }

    private void TryPlaceBetExact(int side) {
        PlaceBet(side, false);
    }

    private void TryPlaceBetGreaterThan(int side) {
        PlaceBet(side, true);
    }

    private void PlaceBet(int side, bool greaterThan) {
        if (waitingForResult)
            return;

        placedBet = currencyManager.currentBet;

        if (placedBet <= 0 || currencyManager.Balance < placedBet) {
            resultText.text = "NOT ENOUGH MONEY TO PLACE BET";
            return;
        }

        currencyManager.RemoveMoney(placedBet);
        diceRoller.ResetValue();

        chosenSide = side;
        isGreaterThanBet = greaterThan;
        waitingForResult = true;

        Debug.Log(
            greaterThan
            ? $"Player bet {placedBet} on > {chosenSide}"
            : $"Player bet {placedBet} on {chosenSide}"
        );

        StartCoroutine(WaitForDiceResult());
    }

    private IEnumerator WaitForDiceResult() {
        yield return new WaitUntil(() => diceRoller.GetCurrentValue() != 0);

        int result = diceRoller.GetCurrentValue();

        bool win =
            isGreaterThanBet
                ? result > chosenSide
                : result == chosenSide;

        if (win) {
            float multiplier = isGreaterThanBet
                ? Random.Range(1.3f, 1.5f)
                : 2f;

            int winAmount = Mathf.RoundToInt(placedBet * multiplier);
            currencyManager.AddMoney(winAmount);

            resultText.text = $"WIN! Rolled {result}, won {winAmount},\n bet again and win";
        } else {
            resultText.text = $"LOSE! Rolled {result}, lost {placedBet},\n bet again and win";
        }

        chosenSide = -1;
        placedBet = 0;
        isGreaterThanBet = false;
        waitingForResult = false;
    }
}
