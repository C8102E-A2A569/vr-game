using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorGrabProxy : MonoBehaviour
{
    [SerializeField] private Transform doorRoot;

    private XRGrabInteractable grabInteractable;
    private Transform interactorAttachTransform;
    private Vector3 localGrabOffset;
    private Quaternion localGrabRotation;
    private bool isGrabbed;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (doorRoot == null && transform.parent != null)
        {
            doorRoot = transform.parent;
        }
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

    private void LateUpdate()
    {
        if (!isGrabbed || interactorAttachTransform == null || doorRoot == null)
        {
            return;
        }

        doorRoot.rotation = interactorAttachTransform.rotation * localGrabRotation;
        doorRoot.position = interactorAttachTransform.position - doorRoot.rotation * localGrabOffset;
    }

    private void HandleSelectEntered(SelectEnterEventArgs args)
    {
        if (doorRoot == null)
        {
            return;
        }

        interactorAttachTransform = args.interactorObject.GetAttachTransform(grabInteractable);
        localGrabOffset = doorRoot.InverseTransformPoint(interactorAttachTransform.position);
        localGrabRotation = Quaternion.Inverse(interactorAttachTransform.rotation) * doorRoot.rotation;
        isGrabbed = true;
    }

    private void HandleSelectExited(SelectExitEventArgs args)
    {
        isGrabbed = false;
        interactorAttachTransform = null;
    }
}
