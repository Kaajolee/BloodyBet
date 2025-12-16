using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BloodBankVisualizer : MonoBehaviour
{
    public CurrencyManager currencyManager;

    public BloodBankLogic logic;
    public BooldBankSpikes bankSpikes;
    public TextMeshProUGUI depositedText;
    public TextMeshProUGUI neededText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI timeText;
    public GameObject gameOverUi;
    public Button gameOverButton;

    private HandController handInside = null;

    public float holdTimeToDeposit = 1.5f;
    private float holdTimer = 0f;

    public float roundTime = 120f; // seconds per round
    private float timer;

    private void Start()
    {
        gameOverUi.SetActive(false);
        timer = roundTime;
        UpdateUI();

        logic.OnGoalReached += () =>
        {
            //logic.DepositBlood(logic.bloodNeededThisRound);
            //logic.DoubleGoal();
            //logic.StartRound();
            currencyManager.AddMoney(logic.bloodNeededThisRound / 3);
            UpdateUI();
            ResetTimer();
        };

        logic.OnRoundFailed += () =>
        {
            GameOver();
        };

        gameOverButton.onClick.AddListener(() => logic.RestartScene());
        currencyManager.OnBalanceZero.AddListener(() => GameOver());
    }

    private void Update()
    {
        if (logic.currentRoundState != RoundState.Failed)
            HandleTimer();

        if (handInside != null)
        {
            // detect fist (adjust based on your VR hand values)
            bool fistClosed = handInside.GetIndexValue > 0.85f &&
                             handInside.GetThumbValue > 0.85f &&
                             handInside.GetThreeFingersValue > 0.85f;


            if (fistClosed)
            {
                holdTimer += Time.deltaTime;

                if (holdTimer >= holdTimeToDeposit)
                {
                    PerformDeposit();
                    holdTimer = 0f;
                    
                }
            }
            else
            {
                holdTimer = 0f;
            }
        }
    }

    private void HandleTimer()
    {
        timer -= Time.deltaTime;
        //timeText.text = "Time: " + timer.ToString("F1");
        timeText.text = "Time: " + TimeSpan.FromSeconds(timer).ToString(@"mm\:ss");
        if (timer <= 0f)
        {
            logic.OnRoundFailed?.Invoke();
        }
    }

    private void ResetTimer()
    {
        timer = roundTime;
        timeText.text = "Time: " + timer.ToString("F1");
    }

    private void PerformDeposit()
    {

        int selectedBlood = currencyManager.currentBet; // YOU implement this


        int accepted = logic.DepositBlood(selectedBlood);

        if (accepted > 0)
        {
            statusText.text = $"Deposited {accepted} blood.";

            if (logic.GoalReached())
            {
                statusText.text = "Required blood reached!";
            }

            UpdateUI();
            currencyManager.RemoveMoney(selectedBlood);
        }
        else
        {
            statusText.text = "Cannot deposit more!";
        }
    }

    private void UpdateUI()
    {
        depositedText.text = "Deposited: " + logic.bloodDeposited;
        neededText.text = "Needed: " + logic.bloodNeededThisRound;
    }

    public void GameOver()
    {
        gameOverUi.SetActive(true);
        timeText.text = "Game Over";
    }

    private void OnTriggerEnter(Collider other)
    {
       // Debug.Log(other.ToString());
        HandController hand = other.GetComponentInParent<HandController>();
        if (hand != null)
        {
            handInside = hand;
            StartCoroutine(bankSpikes.ScaleSpikes(true));
            statusText.text = "Hold fist to deposit...";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log(other.ToString());
        HandController hand = other.GetComponentInParent<HandController>();
        if (hand != null && handInside == hand)
        {
            handInside = null;
            StartCoroutine(bankSpikes.ScaleSpikes(false));
            statusText.text = "";
            holdTimer = 0;
        }
    }
}
