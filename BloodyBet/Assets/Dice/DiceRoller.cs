using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DiceRoller : MonoBehaviour {
    public float randomTorqueStrength = 2f;
    public float stopThreshold = 0.005f;
    public float minStableTime = 0.3f;

    private Rigidbody rb;
    private XRGrabInteractable grab;

    private bool checking = false;
    private float stillTimer = 0f;

    private int currentValue = 0;

    void Start() {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        grab.selectExited.AddListener(OnReleased);
    }

    private void OnReleased(SelectExitEventArgs args) {
        AddRandomSpin();

        if (!checking)
            StartCoroutine(CheckStopped());
    }

    void AddRandomSpin() {
        Vector3 randomTorque = Random.insideUnitSphere * randomTorqueStrength;
        rb.AddTorque(randomTorque, ForceMode.Impulse);
    }

    private System.Collections.IEnumerator CheckStopped() {
        checking = true;

        yield return new WaitForSeconds(0.25f);

        stillTimer = 0f;

        while (true) {
            float vel = rb.linearVelocity.sqrMagnitude;
            float ang = rb.angularVelocity.sqrMagnitude;

            if (vel < stopThreshold && ang < stopThreshold)
                stillTimer += Time.deltaTime;
            else
                stillTimer = 0f;

            if (stillTimer >= minStableTime)
                break;

            yield return null;
        }

        currentValue = GetTopFaceValue();
        Debug.Log("Dice result: " + currentValue);

        checking = false;
    }

    private int GetTopFaceValue() {
        Vector3 up = Vector3.up;

        float bestDot = -999f;
        int bestIndex = -1;

        foreach (Transform face in transform) {
            Vector3 dir = (face.position - transform.position).normalized;
            float dot = Vector3.Dot(dir, up);

            if (dot > bestDot) {
                bestDot = dot;
                int.TryParse(face.name, out bestIndex);
            }
        }

        return bestIndex;
    }

    public int GetCurrentValue() {
        return currentValue;
    }
}
