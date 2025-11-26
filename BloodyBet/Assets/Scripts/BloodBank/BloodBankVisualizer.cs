using TMPro;
using UnityEngine;

public class BloodBankVisualizer : MonoBehaviour
{
    public CurrencyManager currencyManager;

    public BloodBankLogic logic;

    public TextMeshProUGUI depositedText;
    public TextMeshProUGUI neededText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI timeText;

    public Collider depositSlotCollider;
    private HandController handInside = null;

    public float holdTimeToDeposit = 1.5f;
    private float holdTimer = 0f;

    public float roundTime = 120f; // seconds per round
    private float timer;

    private void Start()
    {
        timer = roundTime;
        UpdateUI();

        logic.OnGoalReached += () =>
        {
            logic.DoubleGoal();
            logic.StartRound();
            ResetTimer();
        };

        logic.OnRoundFailed += () =>
        {
            logic.RestartScene();
        };
    }

    private void Update()
    {
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
        timeText.text = "Time: " + timer.ToString("F1");

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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.ToString());
        HandController hand = other.GetComponentInParent<HandController>();
        if (hand != null)
        {
            handInside = hand;
            statusText.text = "Hold fist to deposit...";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.ToString());
        HandController hand = other.GetComponentInParent<HandController>();
        if (hand != null && handInside == hand)
        {
            handInside = null;
            statusText.text = "";
            holdTimer = 0;
        }
    }
}
