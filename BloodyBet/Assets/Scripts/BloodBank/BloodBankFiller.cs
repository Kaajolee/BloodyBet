using UnityEngine;

public class VaseFiller : MonoBehaviour
{
    [Header("Material using the VaseFill shader")]
    public Material fillMaterial;

    [Range(0f, 1f)]
    public float fillAmount = 0f;

    [SerializeField] private float animationLerpDuration = 0.4f;

    private float minY;
    private float maxY;

    // Lerp animation
    private float lerpStartValue;
    private float lerpTargetValue;
    private float lerpDuration;
    private float lerpTimer;
    private bool lerping = false;

    void Start()
    {
        if (fillMaterial == null)
        {
            Debug.LogError("VaseFiller: Assign a material using the VaseFill shader.");
            return;
        }

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogError("VaseFiller: No mesh found.");
            return;
        }

        Bounds bounds = mf.sharedMesh.bounds;

        // Bottom pivot is at minY, top at maxY
        minY = bounds.min.y;
        maxY = bounds.max.y;

        fillMaterial.SetFloat("_MinY", minY);
        fillMaterial.SetFloat("_MaxY", maxY);
        fillMaterial.SetFloat("_FillAmount", fillAmount);
    }

    void Update()
    {
        /*if (fillMaterial == null) return;

        // Example: increase fill with SPACEBAR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float newFill = Mathf.Clamp01(fillAmount + 0.1f);
            SetFillAmount(newFill);
        }*/

        // Animate fill over time
        if (lerping)
        {
            lerpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(lerpTimer / lerpDuration);

            fillAmount = Mathf.Lerp(lerpStartValue, lerpTargetValue, t);
            fillMaterial.SetFloat("_FillAmount", fillAmount);

            if (t >= 1f)
                lerping = false;
        }
    }

    /// <summary>
    /// Instantly sets fill amount.
    /// </summary>
    public void SetFillInstant(float value)
    {
        fillAmount = Mathf.Clamp01(value);
        fillMaterial.SetFloat("_FillAmount", fillAmount);
    }

    /// <summary>
    /// Smoothly animates the fill to a target.
    /// </summary>
    public void SetFillAmount(float target)
    {
        target = Mathf.Clamp01(target);

        lerpStartValue = fillAmount;
        lerpTargetValue = target;
        lerpDuration = Mathf.Max(0.00009f, animationLerpDuration);
        lerpTimer = 0f;
        lerping = true;
    }
}
