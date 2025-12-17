using System.Collections;
using UnityEngine;
using TMPro; // TMP namespace

public class SlotMachine_V2 : MonoBehaviour
{
    public static SlotMachine_V2 Instance { get; private set; }

    public Reel[] reels;
    public Sprite[] symbols;

    [Header("Result Text (TextMeshPro)")]
    public TMP_Text resultText;

    [Header("Win/Lose Visuals")]
    [SerializeField] private Material[] indicatorMaterials; // size = 3
    [SerializeField] private float colorInterval = 0.2f;
    [SerializeField] private float effectDuration = 2f;

    private Color[] originalColors;

    public float spinTime = 2f;
    public float delayBetweenReels = 0.5f;

    public CurrencyManager currencyManager;

    private bool isSpinning = false;

    void Awake()
    {
        Instance = this;

        if (resultText != null)
            resultText.text = "Spin and Win";

        originalColors = new Color[indicatorMaterials.Length];
        for (int i = 0; i < indicatorMaterials.Length; i++)
            originalColors[i] = indicatorMaterials[i].color;
    }

    public Sprite GetRandomSymbol()
    {
        return symbols[Random.Range(0, symbols.Length)];
    }

    public void StartSpin()
    {
        if (isSpinning) return;
        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;

        // Store initial rotations of all reels
        Quaternion[] initialRotations = new Quaternion[reels.Length];
        for (int i = 0; i < reels.Length; i++)
            initialRotations[i] = reels[i].transform.localRotation;

        // Random number of full spins for each reel (1–3 spins)
        int[] fullSpins = new int[reels.Length];
        for (int i = 0; i < reels.Length; i++)
            fullSpins[i] = Random.Range(1, 4); // 1, 2, or 3 full rotations

        // Start the spinning effect text
        StartCoroutine(SpinTextEffect());

        // Start reel logic
        foreach (var reel in reels)
            reel.StartSpin();

        float elapsed = 0f;

        while (elapsed < spinTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spinTime;

            // Use ease-out cubic for natural deceleration
            float ease = 1f - Mathf.Pow(1f - t, 3f);

            for (int i = 0; i < reels.Length; i++)
            {
                float angle = 360f * fullSpins[i] * ease;
                reels[i].transform.localRotation = initialRotations[i] * Quaternion.Euler(0f, angle, 0f);
            }

            yield return null;
        }

        // Ensure all reels return exactly to their initial rotation
        for (int i = 0; i < reels.Length; i++)
            reels[i].transform.localRotation = initialRotations[i];

        // Stop reel logic
        foreach (var reel in reels)
        {
            reel.StopSpin();
            yield return new WaitForSeconds(delayBetweenReels);
        }

        yield return new WaitForSeconds(0.2f);

        // Display final result
        CheckResult();

        isSpinning = false;
    }


    private IEnumerator SpinTextEffect()
    {
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

    private void CheckResult()
    {
        Sprite s0 = reels[0].GetCurrentSprite();
        Sprite s1 = reels[1].GetCurrentSprite();
        Sprite s2 = reels[2].GetCurrentSprite();

        StopAllCoroutines(); // stop previous color effects
        StartCoroutine(ResetMaterials()); // safety reset

        if (s0 == s1 && s0 == s2)
        {
            if (resultText != null) resultText.text = "JACKPOT!";
            Debug.Log("🎉 JACKPOT!");
            currencyManager.AddMoney(currencyManager.currentBet * 7);

            StartCoroutine(JackpotEffect());
        }
        else
        {
            if (resultText != null) resultText.text = "LOSER!";
            Debug.Log("No match");
            currencyManager.RemoveMoney(currencyManager.currentBet);

            SetAllMaterials(Color.red);
            StartCoroutine(ResetMaterialsDelayed());
        }
    }

    private IEnumerator JackpotEffect()
    {
        float elapsed = 0f;

        while (elapsed < effectDuration)
        {
            for (int i = 0; i < indicatorMaterials.Length; i++)
            {
                indicatorMaterials[i].color = Random.ColorHSV(
                    0f, 1f,
                    0.8f, 1f,
                    0.8f, 1f
                );
            }

            elapsed += colorInterval;
            yield return new WaitForSeconds(colorInterval);
        }

        ResetMaterialsImmediate();
    }

    private void SetAllMaterials(Color color)
    {
        for (int i = 0; i < indicatorMaterials.Length; i++)
            indicatorMaterials[i].color = color;
    }

    private IEnumerator ResetMaterialsDelayed()
    {
        yield return new WaitForSeconds(effectDuration);
        ResetMaterialsImmediate();
    }

    private IEnumerator ResetMaterials()
    {
        yield return null;
        ResetMaterialsImmediate();
    }

    private void ResetMaterialsImmediate()
    {
        for (int i = 0; i < indicatorMaterials.Length; i++)
            indicatorMaterials[i].color = originalColors[i];
    }




}