using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class DoorKinematicOnGrab : MonoBehaviour
{
    [SerializeField] private Rigidbody doorRigidbody;
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(HandleSelectEntered);
        grabInteractable.selectExited.AddListener(HandleSelectExited);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(HandleSelectEntered);
        grabInteractable.selectExited.RemoveListener(HandleSelectExited);
    }

    private void Start()
    {
        if (doorRigidbody != null)
        {
            doorRigidbody.isKinematic = true;
            doorRigidbody.velocity = Vector3.zero;
            doorRigidbody.angularVelocity = Vector3.zero;
        }
    }

    private void HandleSelectEntered(SelectEnterEventArgs args)
    {
        if (doorRigidbody == null)
        {
            return;
        }

        doorRigidbody.isKinematic = false;
    }

    private void HandleSelectExited(SelectExitEventArgs args)
    {
        if (doorRigidbody == null)
        {
            return;
        }

        doorRigidbody.isKinematic = true;
        doorRigidbody.velocity = Vector3.zero;
        doorRigidbody.angularVelocity = Vector3.zero;
    }
}
