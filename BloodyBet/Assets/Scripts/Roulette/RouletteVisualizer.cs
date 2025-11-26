using System.Collections;
using TMPro;
using UnityEngine;

public class RouletteVisualizer : MonoBehaviour
{
    public RouletteLogic rouletteLogic;

    public TextMeshProUGUI outputText;
    public Animator wheelAnimator;
    public Animator ballAnimator;

    public float spinDuration = 5f;

    private bool betConfirmed = false;
    private bool ballLanded = false;
    private int landedNumber = -1;

    private void Start()
    {
        StartCoroutine(RouletteFlowRoutine());
    }

    private IEnumerator RouletteFlowRoutine()
    {
        while (true)
        {
            yield return BettingPhase();
            yield return SpinPhase();
            yield return ResultPhase();
            yield return PayoutPhase();
            yield return ResetPhase();
        }
    }

    private IEnumerator BettingPhase()
    {
        outputText.text = "Place your bets!";
        betConfirmed = false;

        rouletteLogic.BeginBetting();

        // Enable UI or VR chip placement
        EnableBetInteraction(true);

        while (!betConfirmed)
            yield return null;

        EnableBetInteraction(false);
        yield return new WaitForSeconds(0.5f);
    }

    public void ConfirmBet()
    {
        betConfirmed = true;
    }

    private IEnumerator SpinPhase()
    {
        outputText.text = "No more bets!";
        ballLanded = false;

       // wheelAnimator.SetTrigger("Spin");
        //ballAnimator.SetTrigger("Spin");

        // logic chooses final number NOW, visual catches up later
        landedNumber = rouletteLogic.SpinWheel();

        yield return new WaitForSeconds(spinDuration);
    }

    private IEnumerator ResultPhase()
    {
        outputText.text = "Ball slowing down...";

        // ball animation calls this at the right time
        while (!ballLanded)
            yield return null;

        outputText.text = $"Winning number: {landedNumber}";
        yield return new WaitForSeconds(1f);
    }

    public void OnBallLanded() // called by animation event
    {
        ballLanded = true;
    }

    private IEnumerator PayoutPhase()
    {
        int payout = rouletteLogic.CalculatePayout(landedNumber);

        if (payout > 0)
            outputText.text = $"You won {payout}!";
        else
            outputText.text = $"You lost.";

        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator ResetPhase()
    {
        outputText.text = "Resetting table...";

        rouletteLogic.ResetTable();
        ResetVisuals();

        yield return new WaitForSeconds(1f);
    }

    private void EnableBetInteraction(bool enable)
    {
        // Activate chip buttons, VR chip system, etc.
    }

    private void ResetVisuals()
    {
        // move ball back to holder, reset animations, etc.
    }
}
