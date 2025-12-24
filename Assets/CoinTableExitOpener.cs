using UnityEngine;

public class CoinTableExitOpener : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform coin;
    [SerializeField] private Collider tableBounds;
    [SerializeField] private Transform safeDoor;
    [SerializeField] private Animator safeAnimator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private float openVolume = 1f;

    [Header("Door Rotation")]
    [SerializeField] private Vector3 openAxis = Vector3.up;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private string animatorTrigger = "Open";

    private bool hasOpened;
    private Quaternion doorClosedRotation;

    private void Awake()
    {
        ResolveReferences();
        if (safeDoor != null)
        {
            doorClosedRotation = safeDoor.localRotation;
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

    private void Reset()
    {
        ResolveReferences();
    }

    private void ResolveReferences()
    {
        if (coin == null)
        {
            coin = transform;
        }

        if (tableBounds == null)
        {
            GameObject tableObject = GameObject.Find("СТОЛ");
            if (tableObject != null)
            {
                tableBounds = tableObject.GetComponent<Collider>();
            }
        }

        if (safeAnimator == null && safeDoor == null)
        {
            SafeInteraction safeInteraction = FindObjectOfType<SafeInteraction>();
            if (safeInteraction != null)
            {
                safeAnimator = safeInteraction.GetComponent<Animator>();
                if (safeAnimator == null)
                {
                    safeDoor = safeInteraction.transform;
                }
            }
        }
    }

    private void Update()
    {
        if (hasOpened || coin == null || tableBounds == null)
        {
            return;
        }

        if (!tableBounds.bounds.Contains(coin.position))
        {
            OpenSafeDoor();
        }
    }

    private void OpenSafeDoor()
    {
        hasOpened = true;

        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound, openVolume);
        }

        if (safeAnimator != null)
        {
            safeAnimator.SetTrigger(animatorTrigger);
            return;
        }

        if (safeDoor != null)
        {
            safeDoor.localRotation = doorClosedRotation * Quaternion.AngleAxis(openAngle, openAxis.normalized);
        }
    }
}
