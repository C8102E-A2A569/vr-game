using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodeLock : MonoBehaviour
{
    public TMP_Text inputField; // Показывает набранный код
    public string correctCode = "1234"; // Пример кода
    public Animator safeAnimator; // Аниматор сейфа
    public GameObject panel; // CodePanel
    public GameObject safeObject; // сейф-объект (если надо выключить его)
    public SafeInteraction safeInteraction;

    private string currentInput = "";

    public void AddDigit(string digit)
    {
        if (currentInput.Length >= 6) return;
        currentInput += digit;
        inputField.text = currentInput;
    }

    public void SubmitCode()
    {
        if (currentInput == correctCode)
        {
            Debug.Log("Correct code");
            safeAnimator.SetTrigger("Open");
            ClosePanel();
            if (safeInteraction != null)
            {
                safeInteraction.UnlockSafe();
            }
            else
            {
                Debug.LogWarning("SafeInteraction не назначен в CodeLock!");
                ClosePanel(); // fallback
            }

        }
        else
        {
            Debug.Log("Wrong code");
            currentInput = "";
            inputField.text = "";
        }
    }

    public void ClosePanel()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        panel.SetActive(false);
    }
}
