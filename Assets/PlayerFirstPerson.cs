using UnityEngine;

public class PlayerFirstPerson : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public Transform cameraTransform;

    [Header("Аудио шагов")]
    public AudioSource footstepAudioSource;
    public AudioClip footstepClip;

    private CharacterController controller;
    private float cameraVerticalRotation = 0f;
    private Vector3 velocity;
    private bool isGrounded;
    public bool controlEnabled = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        if (footstepAudioSource != null)
        {
            footstepAudioSource.clip = footstepClip;
            footstepAudioSource.loop = true;
        }
    }

    void Update()
    {
        if (!controlEnabled) return;
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Move();
        Look();
        ApplyGravityAndJump();
        HandleFootsteps();
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        controller.Move(direction * moveSpeed * Time.deltaTime);
    }
    public void SetControlEnabled(bool enabled)
    {
        controlEnabled = enabled;
    }
    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraVerticalRotation -= mouseY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraVerticalRotation, 0f, 0f);
    }

    void ApplyGravityAndJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleFootsteps()
    {
        if (footstepAudioSource == null || footstepClip == null) return;

        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        if (isGrounded && isMoving)
        {
            if (!footstepAudioSource.isPlaying)
                footstepAudioSource.Play();
        }
        else
        {
            if (footstepAudioSource.isPlaying)
                footstepAudioSource.Pause();
        }
    }
}
