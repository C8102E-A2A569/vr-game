using UnityEngine;

public class SafeInteraction : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    public Camera playerCamera;
    public float interactDistance = 3f;

    [Header("UI панели")]
    public GameObject codeUIPanel;
    public GameObject uiToHideOnCodePanel; // UI, который будет скрыт при открытии кодового замка

    [Header("Аудио")]
    public AudioSource audioSource;
    public AudioClip safeOpenSound;

    private bool isInteracting = false;

    void Update()
    {
        if (isInteracting) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.gameObject == this.gameObject && Input.GetKeyDown(KeyCode.E))
            {
                OpenCodePanel();
            }
        }
    }

    void OpenCodePanel()
    {
        codeUIPanel.SetActive(true);

        if (uiToHideOnCodePanel != null)
            uiToHideOnCodePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f; // Пауза

        isInteracting = true;
    }

    // Этот метод вызывается извне (например, кнопкой в UI), когда ввод правильный
    public void UnlockSafe()
    {
        StartCoroutine(UnlockRoutine());
    }

    private System.Collections.IEnumerator UnlockRoutine()
    {
        // Возвращаем время перед вызовом UI-логики
        Time.timeScale = 1f;

        // Ждём 1 кадр, чтобы Unity успел "разморозить" всё
        yield return null;

        // Скрываем панель с кодом
        if (codeUIPanel != null)
            codeUIPanel.SetActive(false);

        // Включаем скрытую панель
        if (uiToHideOnCodePanel != null)
            uiToHideOnCodePanel.SetActive(true);

        // Проигрываем звук
        if (audioSource != null && safeOpenSound != null)
            audioSource.PlayOneShot(safeOpenSound);

        // Возвращаем управление
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isInteracting = false;
    }
}
