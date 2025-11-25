using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlackjackVisualizer : MonoBehaviour
{
    [SerializeField]
    private BlackjackLogic blackjackLogic;

    [SerializeField] private GameObject playerManager;
    private CurrencyManager currencyManager;

    private int currentBet = 0;

    [SerializeField] private Horizontal3DLayout playerLayout;
    [SerializeField] private Horizontal3DLayout dealerLayout;
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button startButton;

    [SerializeField] private TextMeshProUGUI outputText;

    [SerializeField] private GameObject startUi;
    [SerializeField] private GameObject gameUi;

    private BlackjackStage currentStage = BlackjackStage.Betting;
    private bool playerInputReceived = false;

    void OnEnable()
    {
        blackjackLogic.OnPlayerBust += HandlePlayerBust;
        blackjackLogic.OnDealerBust += HandleDealerBust;
    }

    void OnDisable()
    {
        blackjackLogic.OnPlayerBust -= HandlePlayerBust;
        blackjackLogic.OnDealerBust -= HandleDealerBust;
    }

    void Start()
    {
        hitButton.onClick.AddListener(HitPlayer);
        standButton.onClick.AddListener(StandPlayer);
        startButton.onClick.AddListener(StartGame);

        hitButton.interactable = false;
        standButton.interactable = false;

        currencyManager = playerManager.GetComponent<CurrencyManager>();

        gameUi.SetActive(false);
    }

    private void StartGame()
    {
        startUi.SetActive(false);
        //gameUi.SetActive(true);

        StartCoroutine(GameFlowRoutine());
    }

    private IEnumerator GameFlowRoutine()
    {
        while (true)
        {
            // Simple staged loop
            yield return BettingPhase();
            yield return InitialDealPhase();
            yield return PlayerTurnPhase();
            yield return DealerTurnPhase();
            yield return EvaluationPhase();
            yield return ResetPhase();

        }
    }

    private IEnumerator BettingPhase()
    {
        currentStage = BlackjackStage.Betting;
        Debug.Log("Stage: Betting");
        outputText.text = "Stage: Betting";

        currentBet = currencyManager.currentBet;

        yield return new WaitForSeconds(2f);
    }

    private IEnumerator InitialDealPhase()
    {
        currentStage = BlackjackStage.InitialDeal;
        Debug.Log("Stage: Initial Deal");
        outputText.text = "Stage: Initial Deal";

        blackjackLogic.StartGame();


        blackjackLogic.DealCard(blackjackLogic.DealerHands.First());
        SpawnVisualCard(dealerLayout, blackjackLogic.DealerHands.First().GetLastCard(), true);

        blackjackLogic.DealCard(blackjackLogic.PlayerHands.First());
        SpawnVisualCard(playerLayout, blackjackLogic.PlayerHands.First().GetLastCard(), false);

        blackjackLogic.DealCard(blackjackLogic.DealerHands.First());
        SpawnVisualCard(dealerLayout, blackjackLogic.DealerHands.First().GetLastCard(), false);

        blackjackLogic.DealCard(blackjackLogic.PlayerHands.First());
        SpawnVisualCard(playerLayout, blackjackLogic.PlayerHands.First().GetLastCard(), false);

        yield return new WaitForSeconds(2f);
    }

    private IEnumerator PlayerTurnPhase()
    {
        currentStage = BlackjackStage.PlayerTurn;
        Debug.Log("Stage: Player Turn");
        outputText.text = "Stage: Player Turn";

        hitButton.interactable = true;
        standButton.interactable = true;

        playerInputReceived = false;

        while (playerInputReceived == false)
        {
            yield return null;
        }

        Debug.Log("Player action complete. Exiting Player Turn Phase.");
        hitButton.interactable = false;
        standButton.interactable = false;
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator DealerTurnPhase()
    {
        currentStage = BlackjackStage.DealerTurn;
        Debug.Log("Stage: Dealer Turn");
        outputText.text = "Stage: Dealer Turn";

        var firstCard = dealerLayout.transform.GetChild(0);

        // Set duration for the flip animation
        const float duration = 0.5f;
        float elapsed = 0f;

        // Define start and end rotations
        Quaternion startRotation = Quaternion.Euler(0, 0, 0); // Face Down
        Quaternion endRotation = Quaternion.Euler(-270f, 0, 0);      // Face Up

        while (elapsed < duration)
        {
            // Calculate the interpolation value (t) between 0 and 1
            float t = elapsed / duration;

            // Smoothly rotate the card using Quaternion.Lerp
            firstCard.localRotation = Quaternion.Lerp(startRotation, endRotation, t);

            elapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }


        //dealerLayout.transform.GetChild(0).localRotation = Quaternion.Euler(-270f, 0, 0);

        var total = blackjackLogic.EvaluateHand(blackjackLogic.DealerHands.First());

        while (total < 17)
        {
            HitDealer();
            total = blackjackLogic.EvaluateHand(blackjackLogic.DealerHands.First());
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(2f);
    }

    private IEnumerator EvaluationPhase()
    {
        currentStage = BlackjackStage.HandEvaluation;
        Debug.Log("Stage: Evaluate Hands");

        RoundInfo roundResult = blackjackLogic.EvaluateRound();

        switch (roundResult.Outcome)
        {
            case RoundOutcome.PlayerWin:
                Debug.Log($"Player won: {roundResult.PlayerScore} vs {roundResult.DealerScore}");
                outputText.text = $"Player won: {roundResult.PlayerScore} vs {roundResult.DealerScore}";
                currencyManager.AddMoney(currentBet);
                break;
            case RoundOutcome.DealerWin:
                Debug.Log($"Dealer won: {roundResult.DealerScore} vs {roundResult.PlayerScore}");
                outputText.text = $"Dealer won: {roundResult.DealerScore} vs {roundResult.PlayerScore}";
                currencyManager.RemoveMoney(currentBet);
                break;
            case RoundOutcome.Push:
                Debug.Log($"Push: {roundResult.PlayerScore} vs {roundResult.DealerScore}");
                outputText.text = $"Push: {roundResult.PlayerScore} vs {roundResult.DealerScore}";
                break;
        }

        yield return new WaitForSeconds(2f);
    }

    private IEnumerator ResetPhase()
    {
        currentStage = BlackjackStage.Reset;
        Debug.Log("Stage: Resetting...");
        outputText.text = $"Stage: Resetting...";

        HandleRoundReset();

        yield return new WaitForSeconds(1f);
    }

    private void SpawnVisualCard(Horizontal3DLayout layout, Card cardData, bool flipped)
    {
        GameObject cardObj = Instantiate(cardPrefab, layout.transform);
        cardObj.GetComponentInChildren<TextMeshProUGUI>().text = cardData.ToString();

        if (flipped)
        {
            cardObj.transform.localRotation = Quaternion.Euler(270f, 0, 0);
        }

        layout.AddCardInstance(cardObj);
    }
    private void HandlePlayerBust()
    {
        Debug.Log("Visualizer: Player bust detected!");
        playerInputReceived = true;
    }

    private void HandleDealerBust()
    {
        Debug.Log("Visualizer: Dealer bust detected!");
    }

    private void HandleRoundReset()
    {
        Debug.Log("Visualizer: Resetting table visuals...");
        playerLayout.ClearCards();
        dealerLayout.ClearCards();
    }

    public void HitPlayer()
    {
        if (currentStage == BlackjackStage.PlayerTurn && !playerInputReceived)
        {
            blackjackLogic.DealCard(blackjackLogic.PlayerHands.First());
            SpawnVisualCard(playerLayout, blackjackLogic.PlayerHands.First().GetLastCard(), false);
        }
    }

    void HitDealer()
    {
        if (currentStage == BlackjackStage.DealerTurn)
        {
            blackjackLogic.DealCard(blackjackLogic.DealerHands.First());
            SpawnVisualCard(dealerLayout, blackjackLogic.DealerHands.First().GetLastCard(), false);
        }
    }

    public void StandPlayer()
    {
        if (currentStage == BlackjackStage.PlayerTurn && !playerInputReceived)
        {
            playerInputReceived = true;
        }
    }

    public BlackjackStage GetCurrentStage()
    {
        return currentStage;
    }
}

public enum BlackjackStage
{
    Betting,
    InitialDeal,
    PlayerTurn,
    DealerTurn,
    HandEvaluation,
    Reset,
}
