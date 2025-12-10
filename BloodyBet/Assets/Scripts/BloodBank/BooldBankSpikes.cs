using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BooldBankSpikes : MonoBehaviour
{
    public Transform[] spikes;
    public float scaleUpTime = 1f;
    public Vector3 targetScale = new Vector3(1, 1, 1);

    private Vector3[] originalScales;

    void Start()
    {
        // Save original scales for scale-down
        originalScales = new Vector3[spikes.Length];
        for (int i = 0; i < spikes.Length; i++)
        {
            originalScales[i] = spikes[i].localScale;
        }
    }

    void Update()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            // Example: H scales up, Shift+H scales down
            bool scaleUp = !Keyboard.current.leftShiftKey.isPressed;

            StartCoroutine(ScaleSpikes(scaleUp));
        }
    }

    public IEnumerator ScaleSpikes(bool scaleUp)
    {
        Vector3[] startScales = new Vector3[spikes.Length];
        Vector3[] endScales = new Vector3[spikes.Length];

        // Decide direction
        for (int i = 0; i < spikes.Length; i++)
        {
            startScales[i] = spikes[i].localScale;
            endScales[i] = scaleUp ? targetScale : originalScales[i];
        }

        float elapsed = 0f;

        while (elapsed < scaleUpTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scaleUpTime);

            for (int i = 0; i < spikes.Length; i++)
            {
                spikes[i].localScale = Vector3.Lerp(startScales[i], endScales[i], t);
                Debug.Log("spike skalesd: " + spikes[i].name);
            }

            yield return null;
        }

        // Ensure exact final scale
        for (int i = 0; i < spikes.Length; i++)
        {
            spikes[i].localScale = endScales[i];
        }
    }
}
