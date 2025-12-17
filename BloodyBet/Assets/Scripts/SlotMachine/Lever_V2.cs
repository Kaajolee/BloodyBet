using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // New Input System
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable), typeof(Rigidbody))]
public class Lever_V2 : MonoBehaviour
{
    [Header("Lever Settings")]
    [SerializeField] private float maxLowerAngle = 30f; // Degrees lever can be pulled
    [SerializeField] private float returnSpeed = 5f;    // Speed lever returns
    [SerializeField] private float keyboardSpeed = 60f; // Degrees per second when using keyboard
    [SerializeField] private Transform leverPivot;      // Pivot transform (lever itself)

    private XRGrabInteractable grabInteractable;
    private Quaternion initialRotation;
    private bool isActivated = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        initialRotation = leverPivot.localRotation;

        // Ensure Rigidbody is kinematic so it doesn't fall
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Update()
    {
        // Keyboard control: L key lowers lever (rotate around Y)
        if (Keyboard.current.lKey.isPressed && !isActivated)
        {
            leverPivot.localRotation = Quaternion.RotateTowards(
                leverPivot.localRotation,
                initialRotation * Quaternion.Euler(0, -maxLowerAngle, 0), // Rotate around local Y
                keyboardSpeed * Time.deltaTime
            );
        }

        // Smooth return if not grabbed and not activated
        if (!grabInteractable.isSelected && !isActivated && !Keyboard.current.lKey.isPressed)
        {
            leverPivot.localRotation = Quaternion.Slerp(
                leverPivot.localRotation,
                initialRotation,
                Time.deltaTime * returnSpeed
            );
        }

        // Check if lever pulled far enough
        float angle = Quaternion.Angle(initialRotation, leverPivot.localRotation);
        if (!isActivated && angle >= maxLowerAngle)
        {
            isActivated = true;
            OnLeverPulled();
            StartCoroutine(ReturnLever());
        }
    }

    private void OnLeverPulled()
    {
        Debug.Log("Lever activated!");
        SlotMachine_V2.Instance.StartSpin();
    }

    private IEnumerator ReturnLever()
    {
        yield return new WaitForSeconds(0.5f);
        isActivated = false;
    }
}
