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

    // EVENTS (Visualizer will subscribe)
    public System.Action<int, int> OnBloodChanged;   // current, needed
    public System.Action<float> OnTimerChanged;      // remaining time
    public System.Action OnGoalReached;
    public System.Action OnRoundFailed;

    private void Start()
    {
        StartRound();
    }

    private void Update()
    {
        if (!IsRoundActive)
            return;

        timer -= Time.deltaTime;
        OnTimerChanged?.Invoke(timer);

        if (timer <= 0f)
        {
            // TIME OUT ? FAIL
            IsRoundActive = false;
            OnRoundFailed?.Invoke();
        }
    }

    // ------------------------------------------------------------
    // BLOOD DEPOSIT
    // ------------------------------------------------------------
    public int DepositBlood(int amount)
    {
        if (!IsRoundActive) return 0;

        int accepted = Mathf.Min(amount, bloodNeededThisRound - bloodDeposited);

        bloodDeposited += accepted;

        OnBloodChanged?.Invoke(bloodDeposited, bloodNeededThisRound);

        if (bloodDeposited >= bloodNeededThisRound)
        {
            IsRoundActive = false;
            OnGoalReached?.Invoke();
        }

        return accepted;
    }

    public bool GoalReached()
    {
        return bloodDeposited >= bloodNeededThisRound;
    }

    // ------------------------------------------------------------
    // ROUND MANAGEMENT
    // ------------------------------------------------------------
    public void StartRound()
    {
        bloodDeposited = 0;
        timer = roundTimeSeconds;
        IsRoundActive = true;

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
        bloodNeededThisRound *= 2;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
