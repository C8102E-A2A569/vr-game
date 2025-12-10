using UnityEngine;
using UnityEngine.AI;

public class InteractionTrigger : MonoBehaviour
{
    [Header("UI и цели")]
    public GameObject uiPanel;
    public Transform targetPoint;
    public GameObject objectToDisable;
    public GameObject objectToEnable;

    [Header("Взаимодействие")]
    public float interactionDistance = 3f;
    public float interactionAngle = 30f;
    public Transform playerCamera; // Камера игрока (или голова)

    [Header("Навигация")]
    public float stopDistance = 1.5f;

    [Header("Звук удара")]
    public AudioClip hitSound;
    public AudioSource audioSource;

    private Animator animator;
    private NavMeshAgent agent;

    private bool isInteracting = false;
    private bool hasArrived = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (uiPanel != null)
            uiPanel.SetActive(false);

        if (audioSource == null && hitSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isInteracting && IsLookingAtTarget())
        {
            StartInteraction();
        }

        if (isInteracting && !hasArrived)
        {
            float distance = Vector3.Distance(transform.position, targetPoint.position);
            if (distance <= stopDistance)
            {
                ArrivedAtTarget();
            }
        }
    }

    bool IsLookingAtTarget()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            return hit.collider.gameObject == this.gameObject;
        }
        return false;
    }

    void StartInteraction()
    {
        isInteracting = true;
        uiPanel.SetActive(true);
        Invoke(nameof(StartRunning), 2f); // Ждём 2 секунды — показать панель
    }

    void StartRunning()
    {
        uiPanel.SetActive(false);
        animator.SetInteger("State", 1); // Бег
        agent.SetDestination(targetPoint.position);
    }

    void ArrivedAtTarget()
    {
        hasArrived = true;
        agent.isStopped = true;
        animator.SetInteger("State", 2); // Удар

        // Проигрываем звук удара
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        StartCoroutine(WaitForAnimationToEnd());
    }

    System.Collections.IEnumerator WaitForAnimationToEnd()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (objectToDisable != null)
            objectToDisable.SetActive(false);

        if (objectToEnable != null)
            objectToEnable.SetActive(true);

        animator.SetInteger("State", 0); // Стоит
    }
}
