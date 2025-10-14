using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(Collider))]
public class Lever : MonoBehaviour {
    [Header("Slot Machine Reference")]
    [SerializeField] private SlotMachine slotMachine;

    [Header("Lever Animation")]
    [SerializeField] private float pullAngle = 30f;       // kiek laipsnių lenkiasi rankena žemyn
    [SerializeField] private float returnDuration = 0.4f; // animacijos greitis

    [Header("Grab Settings")]
    [SerializeField] private bool allowGrabRotation = true; // ar ranka gali sukti rankeną

    [Header("Threshold")]
    [SerializeField] private float pullThresholdDegrees = 20f; // laipsniai, per kuriuos spin paleidžiamas

    private Quaternion startRotation;
    private bool pulled = false;
    private XRGrabInteractable grab;

    void Start() {
        startRotation = transform.localRotation;

        grab = GetComponent<XRGrabInteractable>();
        grab.trackPosition = false;             // objekto pozicija lieka vietoje
        grab.trackRotation = allowGrabRotation; // ranka gali pasukti rankeną
        grab.throwOnDetach = false;             // kad nebūtų klaidų su kinematic Rigidbody
    }

    void Update() {
        if (!pulled) {
            // patikriname, kiek rankena pasukta nuo startRotation
            float angle = Quaternion.Angle(startRotation, transform.localRotation);
            if (angle >= pullThresholdDegrees) {
                pulled = true;
                StartCoroutine(PlayLeverAndTrigger());
            }
        }
    }

    private IEnumerator PlayLeverAndTrigger() {
        // užtikriname, kad rankena nenukryptų
        Quaternion targetRotation = startRotation * Quaternion.Euler(-pullAngle, 0f, 0f);

        // Animacija žemyn (jeigu norisi pabrėžti lenkimą)
        float elapsed = 0f;
        while (elapsed < returnDuration) {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / returnDuration);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, t);
            yield return null;
        }

        // Paleidžiame slot machine
        if (slotMachine != null)
            slotMachine.StartSpin();

        // Animacija atgal į startRotation
        elapsed = 0f;
        while (elapsed < returnDuration) {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / returnDuration);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, startRotation, t);
            yield return null;
        }

        transform.localRotation = startRotation;
        pulled = false;
    }
}