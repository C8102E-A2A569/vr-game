using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandleUnlockOnGrab : MonoBehaviour
{
    [SerializeField] private GameObject[] handleObjects;
    [SerializeField] private float unlockDelaySeconds = 2f;
    [SerializeField] private int enabledLayer = 7;
    [SerializeField] private int disabledLayer = 6;

    private XRGrabInteractable grabInteractable;
    private bool hasTriggered;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        SetHandlesLayer(disabledLayer);
    }

    private void OnEnable()
    {
        if (grabInteractable == null)
        {
            return;
        }

        grabInteractable.selectEntered.AddListener(HandleSelectEntered);
    }

    private void OnDisable()
    {
        if (grabInteractable == null)
        {
            return;
        }

        grabInteractable.selectEntered.RemoveListener(HandleSelectEntered);
    }

    private void HandleSelectEntered(SelectEnterEventArgs args)
    {
        if (hasTriggered)
        {
            return;
        }

        hasTriggered = true;
        StartCoroutine(EnableHandlesAfterDelay());
    }

    private IEnumerator EnableHandlesAfterDelay()
    {
        yield return new WaitForSeconds(unlockDelaySeconds);
        SetHandlesLayer(enabledLayer);
    }

    private void SetHandlesLayer(int layer)
    {
        if (handleObjects == null)
        {
            return;
        }

        for (int i = 0; i < handleObjects.Length; i++)
        {
            GameObject handle = handleObjects[i];
            if (handle != null)
            {
                handle.layer = layer;
            }
        }
    }
}
