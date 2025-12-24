using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GravityAfterGrab : MonoBehaviour
{
    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private bool enableGravityOnRelease = true;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (targetRigidbody == null)
        {
            targetRigidbody = GetComponent<Rigidbody>();
        }

        if (targetRigidbody != null)
        {
            targetRigidbody.useGravity = false;
            targetRigidbody.isKinematic = true;
        }
    }

    private void OnEnable()
    {
        if (grabInteractable == null)
        {
            return;
        }

        grabInteractable.selectEntered.AddListener(HandleSelectEntered);
        grabInteractable.selectExited.AddListener(HandleSelectExited);
    }

    private void OnDisable()
    {
        if (grabInteractable == null)
        {
            return;
        }

        grabInteractable.selectEntered.RemoveListener(HandleSelectEntered);
        grabInteractable.selectExited.RemoveListener(HandleSelectExited);
    }

    private void HandleSelectEntered(SelectEnterEventArgs args)
    {
        if (targetRigidbody == null)
        {
            return;
        }

        targetRigidbody.useGravity = false;
        targetRigidbody.isKinematic = false;
    }

    private void HandleSelectExited(SelectExitEventArgs args)
    {
        if (targetRigidbody == null)
        {
            return;
        }

        targetRigidbody.useGravity = enableGravityOnRelease;
        targetRigidbody.isKinematic = false;
    }
}
