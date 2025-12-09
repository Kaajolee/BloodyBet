using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Lever : MonoBehaviour {

    private HingeJoint hinge;

    [SerializeField] private SlotMachine slotMachine;

    [SerializeField] private float leverOutput = 90;
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;
    [SerializeField] private float startingValue;

    void Start() {
        hinge = GetComponent<HingeJoint>();

        if (startingValue >= minValue && startingValue <= maxValue) {
            float rangeFraction = (startingValue - minValue) / (maxValue - minValue);
            float degreeRotation = hinge.limits.min + (hinge.limits.max - hinge.limits.min) * rangeFraction;
            Vector3 worldSpaceHingeAxis = transform.TransformDirection(hinge.axis);
            transform.rotation = Quaternion.AngleAxis(degreeRotation, worldSpaceHingeAxis) * transform.rotation;
        }
    }

    void Update() {
        float leverValue = (hinge.angle - hinge.limits.min) / (hinge.limits.max - hinge.limits.min);
        leverOutput = minValue + (maxValue - minValue) * leverValue;

        StartMachine();
    }

    private void ResetLeverRotation() {
        if (startingValue <= maxValue) {
            float rangeFraction = (startingValue - minValue) / (maxValue - minValue);
            float degreeRotation = hinge.limits.min + (hinge.limits.max - hinge.limits.min) * rangeFraction;
            Vector3 worldSpaceHingeAxis = transform.TransformDirection(hinge.axis);
            transform.rotation = Quaternion.AngleAxis(degreeRotation, worldSpaceHingeAxis) * transform.rotation;
        }
    }

    private void StartMachine() {
        if (leverOutput <= 10) {
            if (slotMachine != null) {
                slotMachine.StartSpin();
                StartCoroutine(ReturnLever());
            }
        }
    }

    private IEnumerator ReturnLever() {
        yield return new WaitForSeconds(0.5f);

        ResetLeverRotation();

        yield return new WaitForSeconds(0.5f);
    }
}
