using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable), typeof(Rigidbody))]
public class Lever_V2 : MonoBehaviour
{
    [Header("Lever Settings")]
    [SerializeField] private float maxLowerAngle = 30f;
    [SerializeField] private float returnSpeed = 5f;
    [SerializeField] private float keyboardSpeed = 60f;
    [SerializeField] private Transform leverPivot;

    [Header("VR Pull Settings")]
    [SerializeField] private float vrFollowSpeed = 20f;

    private XRGrabInteractable grabInteractable;
    private Quaternion initialLeverRotation;
    private bool isActivated = false;

    // Grab tracking
    private IXRSelectInteractor currentInteractor;
    private Vector3 grabStartLocalDir;

    // Lock the lever object's transform (so it never moves)
    private Vector3 lockedWorldPos;
    private Quaternion lockedWorldRot;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Lever pivot rotation baseline
        initialLeverRotation = leverPivot.localRotation;

        // Lock this object's transform so it cannot be carried away
        lockedWorldPos = transform.position;
        lockedWorldRot = transform.rotation;

        // Ensure Rigidbody never moves
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Make grab "input-only": do not move/rotate the grabbed object transform
        grabInteractable.trackPosition = false;
        grabInteractable.trackRotation = false;
        grabInteractable.throwOnDetach = false;

        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
            grabInteractable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    void LateUpdate()
    {
        // Safety net: even if something tries to move it, snap it back
        transform.SetPositionAndRotation(lockedWorldPos, lockedWorldRot);
    }

    void Update()
    {
        if (isActivated) return;

        bool grabbed = grabInteractable.isSelected && currentInteractor != null;
        bool keyboardPull = Keyboard.current != null && Keyboard.current.lKey.isPressed;

        if (grabbed)
        {
            ApplyVRPull();
        }
        else if (keyboardPull)
        {
            leverPivot.localRotation = Quaternion.RotateTowards(
                leverPivot.localRotation,
                initialLeverRotation * Quaternion.Euler(0f, -maxLowerAngle, 0f),
                keyboardSpeed * Time.deltaTime
            );
        }
        else
        {
            leverPivot.localRotation = Quaternion.Slerp(
                leverPivot.localRotation,
                initialLeverRotation,
                Time.deltaTime * returnSpeed
            );
        }

        float pulledAngle = Quaternion.Angle(initialLeverRotation, leverPivot.localRotation);
        if (pulledAngle >= maxLowerAngle)
        {
            isActivated = true;
            OnLeverPulled();
            StartCoroutine(ReturnLever());
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject as IXRSelectInteractor;

        Vector3 handWorldPos = args.interactorObject.transform.position;
        Vector3 dirWorld = handWorldPos - leverPivot.position;
        grabStartLocalDir = leverPivot.InverseTransformDirection(dirWorld).normalized;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        currentInteractor = null;
    }

    private void ApplyVRPull()
    {
        Vector3 handWorldPos = currentInteractor.transform.position;
        Vector3 dirWorld = handWorldPos - leverPivot.position;
        Vector3 dirLocal = leverPivot.InverseTransformDirection(dirWorld).normalized;

        Vector3 a = new Vector3(grabStartLocalDir.x, 0f, grabStartLocalDir.z).normalized;
        Vector3 b = new Vector3(dirLocal.x, 0f, dirLocal.z).normalized;

        if (a.sqrMagnitude < 0.0001f || b.sqrMagnitude < 0.0001f)
            return;

        float signed = Vector3.SignedAngle(a, b, Vector3.up);

        // Pull down is negative rotation -> convert to positive pull amount
        float pullAmount = Mathf.Clamp(-signed, 0f, maxLowerAngle);

        Quaternion target = initialLeverRotation * Quaternion.Euler(0f, -pullAmount, 0f);

        leverPivot.localRotation = Quaternion.Slerp(
            leverPivot.localRotation,
            target,
            Time.deltaTime * vrFollowSpeed
        );
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
