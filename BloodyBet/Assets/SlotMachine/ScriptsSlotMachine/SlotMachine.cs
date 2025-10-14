using System.Collections;
using UnityEngine;
using TMPro; // TMP namespace

public class SlotMachine : MonoBehaviour {
    public static SlotMachine Instance { get; private set; }

    public Reel[] reels;
    public Sprite[] symbols;

    [Header("Result Text (TextMeshPro)")]
    public TMP_Text resultText;

    public float spinTime = 2f;
    public float delayBetweenReels = 0.5f;

    private bool isSpinning = false;

    void Awake() {
        Instance = this;

        // Prieš pirmą spin, rodomas tekstas
        if (resultText != null)
            resultText.text = "Suk ir laimėk";
    }

    public Sprite GetRandomSymbol() {
        return symbols[Random.Range(0, symbols.Length)];
    }

    public void StartSpin() {
        if (isSpinning) return;
        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine() {
        isSpinning = true;

        // Pradeda rodyti spinning efektą
        StartCoroutine(SpinTextEffect());

        foreach (var reel in reels)
            reel.StartSpin();

        yield return new WaitForSeconds(spinTime);

        foreach (var reel in reels) {
            reel.StopSpin();
            yield return new WaitForSeconds(delayBetweenReels);
        }

        yield return new WaitForSeconds(0.2f);

        // Po sustojimo rodomas galutinis rezultatas
        CheckResult();

        isSpinning = false;
    }

    private IEnumerator SpinTextEffect() {
        string pattern = "*******";
        int index = 0;
        while (isSpinning) // <--- rodom tik kol sukasi
        {
            if (resultText != null)
                resultText.text = pattern.Substring(0, index + 1);
            index = (index + 1) % pattern.Length;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void CheckResult() {
        Sprite s0 = reels[0].GetCurrentSprite();
        Sprite s1 = reels[1].GetCurrentSprite();
        Sprite s2 = reels[2].GetCurrentSprite();

        if (s0 == s1 && s0 == s2) {
            if (resultText != null) resultText.text = "JACKPOT!";
            Debug.Log("🎉 JACKPOT!");
        } else {
            if (resultText != null) resultText.text = "LOSER!";
            Debug.Log("No match");
        }
    }
}