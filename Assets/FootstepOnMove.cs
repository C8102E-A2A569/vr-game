using UnityEngine;

public class FootstepOnMove : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private float minSpeed = 0.15f;
    [SerializeField] private bool useKeyboardInput = true;
    [SerializeField] private bool usePositionSpeed = true;

    private Vector3 lastPosition;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        if (audioSource != null)
        {
            audioSource.clip = footstepClip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }

        lastPosition = transform.position;
    }

    private void Update()
    {
        if (audioSource == null || footstepClip == null)
        {
            lastPosition = transform.position;
            return;
        }

        bool isMoving = false;

        if (useKeyboardInput)
        {
            isMoving |= Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        }

        if (usePositionSpeed)
        {
            float distance = Vector3.Distance(transform.position, lastPosition);
            float speed = Time.deltaTime > 0f ? distance / Time.deltaTime : 0f;
            isMoving |= speed >= minSpeed;
        }

        if (isMoving)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }

        lastPosition = transform.position;
    }
}
