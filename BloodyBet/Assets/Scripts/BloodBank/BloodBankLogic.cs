using UnityEngine;
using UnityEngine.SceneManagement;

public class BloodBankLogic : MonoBehaviour
{
    [Header("Goal")]
    public int bloodNeededThisRound = 5000;
    public int bloodDeposited = 0;

    [Header("Timer")]
    public float roundTimeSeconds = 30f;
    private float timer;

    public bool IsRoundActive { get; private set; }

    public RoundState currentRoundState = RoundState.Active;

    public CurrencyManager currencyManager;
    public VaseFiller VaseFiller;

    // EVENTS (Visualizer will subscribe)
    public System.Action<int, int> OnBloodChanged;   // current, needed
    public System.Action<float> OnTimerChanged;      // remaining time
    public System.Action OnGoalReached;
    public System.Action OnRoundFailed;

    private void Start()
    {
        currencyManager.OnBalanceZero.AddListener(() => GoalFailed());
        StartRound();
    }

    private void Update()
    {

        switch (currentRoundState)
        { 
            case RoundState.Active:
                timer -= Time.deltaTime;
                OnTimerChanged?.Invoke(timer);

                if (timer <= 0f)
                {
                    currentRoundState = RoundState.Failed;
                }
                break;

            case RoundState.Reached:
                DoubleGoal();
                StartRound();
                OnGoalReached?.Invoke();
                currentRoundState = RoundState.Active;
                break;

            case RoundState.Failed:
                //RestartScene();
                break;
        }
    }

    // ------------------------------------------------------------
    // BLOOD DEPOSIT
    // ------------------------------------------------------------
    public int DepositBlood(int amount)
    {
        if (currentRoundState != RoundState.Active) return 0;

        int accepted = Mathf.Min(amount, bloodNeededThisRound - bloodDeposited);

        bloodDeposited += accepted;

        OnBloodChanged?.Invoke(bloodDeposited, bloodNeededThisRound);

        float coef = (float)bloodDeposited / (float)bloodNeededThisRound;

        //Debug.Log($"cokeceofj:{coef}   bloodjhgoishds:{bloodDeposited},  needed tihs rmeoub:{bloodNeededThisRound}");

        VaseFiller.SetFillAmount(coef);

        if (bloodDeposited >= bloodNeededThisRound)
        {
            //OnGoalReached?.Invoke();
            currentRoundState = RoundState.Reached;
        }

        return accepted;
    }

    public bool GoalReached()
    {
        return bloodDeposited >= bloodNeededThisRound;
    }

    public void GoalFailed()
    {
        currentRoundState = RoundState.Failed;
    }

    // ------------------------------------------------------------
    // ROUND MANAGEMENT
    // ------------------------------------------------------------
    public void StartRound()
    {
        bloodDeposited = 0;
        timer = roundTimeSeconds;
        //IsRoundActive = true;
        currentRoundState = RoundState.Active;

        OnBloodChanged?.Invoke(bloodDeposited, bloodNeededThisRound);
        OnTimerChanged?.Invoke(timer);
    }

    public void ResetRound(int newGoal)
    {
        bloodNeededThisRound = newGoal;
        StartRound();
    }

    public void DoubleGoal()
    {
        bloodNeededThisRound *= 3;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

public enum RoundState
{
    Active,
    Reached,
    Failed,
}
