using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorHingeGrabber : MonoBehaviour
{
    [SerializeField] private Transform doorRoot;
    [SerializeField] private Transform hinge;
    [SerializeField] private Vector3 hingeAxis = Vector3.up;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private float openSoundAngle = 20f;

    private XRGrabInteractable grabInteractable;
    private Transform interactorAttachTransform;
    private Vector3 startDirection;
    private Quaternion startDoorRotation;
    private Vector3 startDoorPosition;
    private Vector3 startHingePosition;
    private Vector3 startHingeAxis;
    private bool isGrabbed;
    private bool hasPlayedOpenSound;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (doorRoot == null)
        {
            doorRoot = transform;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
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
        if (!isGrabbed || interactorAttachTransform == null || doorRoot == null || hinge == null)
        {
            return;
        }

        Vector3 currentDirection = ProjectOnPlane(interactorAttachTransform.position - startHingePosition, startHingeAxis);
        if (currentDirection.sqrMagnitude < 0.0001f)
        {
            return;
        }

        float angle = SignedAngleOnAxis(startDirection, currentDirection, startHingeAxis);
        Quaternion deltaRotation = Quaternion.AngleAxis(angle, startHingeAxis);
        doorRoot.rotation = deltaRotation * startDoorRotation;
        doorRoot.position = startHingePosition + deltaRotation * (startDoorPosition - startHingePosition);

        if (!hasPlayedOpenSound && Mathf.Abs(angle) >= openSoundAngle)
        {
            PlayOpenSound();
            hasPlayedOpenSound = true;
        }
    }

    private void HandleSelectEntered(SelectEnterEventArgs args)
    {
        if (doorRoot == null || hinge == null)
        {
            return;
        }

        interactorAttachTransform = args.interactorObject.GetAttachTransform(grabInteractable);
        startHingePosition = hinge.position;
        startHingeAxis = hinge.TransformDirection(hingeAxis.normalized);
        startDirection = ProjectOnPlane(interactorAttachTransform.position - startHingePosition, startHingeAxis);
        if (startDirection.sqrMagnitude < 0.0001f)
        {
            startDirection = ProjectOnPlane(transform.position - startHingePosition, startHingeAxis);
        }

        startDoorRotation = doorRoot.rotation;
        startDoorPosition = doorRoot.position;
        isGrabbed = true;
        hasPlayedOpenSound = false;
    }

    private void HandleSelectExited(SelectExitEventArgs args)
    {
        isGrabbed = false;
        interactorAttachTransform = null;
    }

    private void PlayOpenSound()
    {
        if (audioSource == null || openSound == null)
        {
            return;
        }

        audioSource.PlayOneShot(openSound);
    }

    private static Vector3 ProjectOnPlane(Vector3 vector, Vector3 normal)
    {
        return vector - Vector3.Dot(vector, normal) * normal;
    }

    private static float SignedAngleOnAxis(Vector3 from, Vector3 to, Vector3 axis)
    {
        Vector3 cross = Vector3.Cross(from, to);
        float sign = Mathf.Sign(Vector3.Dot(cross, axis));
        float angle = Vector3.Angle(from, to);
        return angle * sign;
    }
}
