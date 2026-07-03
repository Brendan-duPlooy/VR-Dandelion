using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DandelionVRInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private DandelionBloom bloom;

    [SerializeField]
    private XRGrabInteractable grabInteractable;

    private bool isGrabbed;

    private InputDevice rightController;

    private bool triggerPressedLastFrame;
    private bool bPressedLastFrame;

    void Reset()
    {
        bloom = GetComponent<DandelionBloom>();
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void Start()
    {
        rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        bloom.BloomFlower();
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    void Update()
    {
        if (!rightController.isValid)
            rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Reset should work even when not holding the flower.
        CheckResetButton();

        if (!isGrabbed)
            return;

        CheckBlowButton();
    }

    private void CheckBlowButton()
    {
        bool triggerPressed = false;

        if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed))
        {
            if (triggerPressed && !triggerPressedLastFrame)
            {
                bloom.BlowFlower();
            }

            triggerPressedLastFrame = triggerPressed;
        }
    }

    private void CheckResetButton()
    {
        bool bPressed = false;

        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bPressed))
        {
            if (bPressed && !bPressedLastFrame)
            {
                bloom.ResetFlower();
            }

            bPressedLastFrame = bPressed;
        }
    }
}