using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandController : MonoBehaviour
{
    public HandType handType;

    private Animator animator;
    private InputDevice inputDevice;

    bool lookingForDevice = true;

    float indexValue;
    float thumbValue;
    float threeFingersValue;

    float thumbMoveSpeed = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        //inputDevice = GetInputDevice();
    }

    // Update is called once per frame
    void Update()
    {
        if (lookingForDevice)
        {
            inputDevice = GetInputDevice();
            lookingForDevice = false;
        }

        AnimateHand();
    }

    public float GetIndexValue => indexValue;
    public float GetThumbValue => thumbValue;
    public float GetThreeFingersValue => threeFingersValue;

    InputDevice GetInputDevice()
    {
        InputDeviceCharacteristics controllerCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller;

        if (handType == HandType.Left)
        {
            controllerCharacteristics = controllerCharacteristics | InputDeviceCharacteristics.Left;
        }
        else if (handType == HandType.Right)
        {
            controllerCharacteristics = controllerCharacteristics | InputDeviceCharacteristics.Right;
        }

        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, inputDevices);

        if (inputDevices.Count > 0)
        {
            return inputDevices[0];
        }
        else
        {
            //Debug.LogWarning($"No {handType} controller found yet!");
            lookingForDevice = true; // try again next frame
            return default; // return an empty InputDevice
        }
    }

    void AnimateHand()
    {
        inputDevice.TryGetFeatureValue(CommonUsages.trigger, out indexValue);
        inputDevice.TryGetFeatureValue(CommonUsages.grip, out threeFingersValue);

        inputDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouched);
        inputDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out bool secondaryTouch);

        if (primaryTouched || secondaryTouch)
        {
            thumbValue += thumbMoveSpeed;
        }
        else
        {
            thumbValue -= thumbMoveSpeed;
        }

        thumbValue = Mathf.Clamp(thumbValue, 0f, 1f);

        animator.SetFloat("Index", indexValue);
        animator.SetFloat("ThreeFingers", threeFingersValue);
        animator.SetFloat("Thumb", thumbValue);
    }
}

public enum HandType
{
    Left,
    Right,
}
