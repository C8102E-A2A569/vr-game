using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorHandleDriver : MonoBehaviour
{
    [SerializeField] private Transform door;              // Door root to rotate
    [SerializeField] private Transform hingeWorld;        // World-space pivot (empty at hinge)
    [SerializeField] private float minAngle = -120f;      // Closed to open range
    [SerializeField] private float maxAngle = 0f;

    private XRGrabInteractable grab;
    private bool grabbing;
    private Quaternion doorStartRot;
    private Vector3 handleStartDir;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs _)
    {
        grabbing = true;
        doorStartRot = door.rotation;
        handleStartDir = ProjectOnPlane(transform.position - hingeWorld.position, hingeWorld.up).normalized;
    }

    private void OnRelease(SelectExitEventArgs _)
    {
        grabbing = false;
    }

    private void Update()
    {
        if (!grabbing) return;

        Vector3 currentDir = ProjectOnPlane(transform.position - hingeWorld.position, hingeWorld.up).normalized;
        float delta = SignedAngleOnAxis(handleStartDir, currentDir, hingeWorld.up);

        // Apply delta around hinge
        door.rotation = Quaternion.AngleAxis(Mathf.Clamp(delta, minAngle, maxAngle), hingeWorld.up) * doorStartRot;
    }

    private static Vector3 ProjectOnPlane(Vector3 v, Vector3 normal)
    {
        return v - Vector3.Dot(v, normal) * normal;
    }

    private static float SignedAngleOnAxis(Vector3 from, Vector3 to, Vector3 axis)
    {
        Vector3 cross = Vector3.Cross(from, to);
        float sign = Mathf.Sign(Vector3.Dot(cross, axis));
        float angle = Vector3.Angle(from, to);
        return angle * sign;
    }
}
