using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GripHoldInteractable : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float holdTime = 1.0f;
    [SerializeField] private GameObject BlackJackTable;
    private BlackjackVisualizer visualizer;

    private float holdTimer;
    private bool isHeld;
    private bool standTriggered;

    [Header("Visual Feedback")]
    public Image progressImage; // assign a circular UI Image
    public TextMeshProUGUI text;
    public Color defaultColor = Color.white;
    public Color fillColor = Color.softYellow;
    public GameObject InteractionProgressBubble;

    [Header("Optional")]
    public float showProgressDelay = 0.3f;

    void Start()
    {
        visualizer = BlackJackTable.GetComponent<BlackjackVisualizer>();
    }

    public void OnSelectEntered()
    {
        isHeld = true;
        standTriggered = false;
        holdTimer = 0f;

        if (progressImage)
        {
            progressImage.fillAmount = 0f;
            progressImage.color = fillColor;
        }
    }

    public void OnSelectExited()
    {
        // Released before holdTime Hit
        if (isHeld && !standTriggered)
        {
            visualizer.HitPlayer();
            InteractionProgressBubble.SetActive(true);
            ShowHit();
        }

        // Reset
        isHeld = false;
        holdTimer = 0f;

        ResetProgress();
    }

    private void Update()
    {
        //if (!isHeld || visualizer.GetCurrentStage() != BlackjackStage.PlayerTurn)
        //    return;

        if (!isHeld || visualizer.GetCurrentStage() != BlackjackStage.PlayerTurn)
        {
            if (InteractionProgressBubble.activeSelf)
            {
                InteractionProgressBubble.SetActive(false);
            }
            return;
        }

        holdTimer += Time.deltaTime;

        //float progress = Mathf.Clamp01(holdTimer / holdTime);

        //if (progressImage)
        //    progressImage.fillAmount = progress;

        // Show progress circle only after short delay
        if (progressImage && holdTimer >= showProgressDelay)
        {
            InteractionProgressBubble.SetActive(true);
            text.text = "Stand";
            float progress = Mathf.Clamp01((holdTimer - showProgressDelay) / (holdTime - showProgressDelay));
            progressImage.fillAmount = progress;
        }

        // Hold long enough Stand
        if (!standTriggered && holdTimer >= holdTime)
        {
            standTriggered = true;
            visualizer.StandPlayer();
            ResetProgress();
            InteractionProgressBubble.SetActive(false);
        }
    }
    private void ResetProgress()
    {
        isHeld = false;
        holdTimer = 0f;

        if (progressImage)
        {
            progressImage.fillAmount = 0f;
            progressImage.color = defaultColor;
        }
    }

    private void ShowHit()
    {
        text.text = "Hit";
        progressImage.fillAmount = 1;
        progressImage.color = Color.black;
    }

}
