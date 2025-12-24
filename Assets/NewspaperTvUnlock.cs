using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NewspaperTvUnlock : MonoBehaviour
{
    [SerializeField] private XRBaseInteractable newspaperInteractable;
    [SerializeField] private XRBaseInteractable tvInteractable;
    [SerializeField] private TVController tvController;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private string message = "Этого не может быть.. нужно проверить новости...";

    private bool hasTriggered;

    private void Awake()
    {
        if (newspaperInteractable == null)
        {
            newspaperInteractable = GetComponent<XRBaseInteractable>();
        }

        if (tvInteractable == null)
        {
            GameObject screenObject = GameObject.Find("Экран");
            if (screenObject != null)
            {
                tvInteractable = screenObject.GetComponent<XRBaseInteractable>();
            }
        }

        if (tvController == null && tvInteractable != null)
        {
            tvController = tvInteractable.GetComponent<TVController>();
        }

        if (dialogText == null)
        {
            TMP_Text[] texts = FindObjectsOfType<TMP_Text>(true);
            foreach (TMP_Text text in texts)
            {
                if (text.text.Contains("Почини"))
                {
                    dialogText = text;
                    break;
                }
            }
        }
    }

    private void Start()
    {
        SetTvInteractable(false);
        HideDialogMessage();
    }

    private void OnEnable()
    {
        if (newspaperInteractable != null)
        {
            newspaperInteractable.selectEntered.AddListener(HandleSelectEntered);
        }

        if (tvInteractable != null)
        {
            tvInteractable.selectEntered.AddListener(HandleTvSelected);
        }
    }

    private void OnDisable()
    {
        if (newspaperInteractable != null)
        {
            newspaperInteractable.selectEntered.RemoveListener(HandleSelectEntered);
        }

        if (tvInteractable != null)
        {
            tvInteractable.selectEntered.RemoveListener(HandleTvSelected);
        }
    }

    private void HandleSelectEntered(SelectEnterEventArgs args)
    {
        if (hasTriggered)
        {
            return;
        }

        hasTriggered = true;

        ShowDialogMessage();

        SetTvInteractable(true);
    }

    private void HandleTvSelected(SelectEnterEventArgs args)
    {
        SetTvInteractable(false);
        HideDialogMessage();
    }

    private void SetTvInteractable(bool enabled)
    {
        if (tvInteractable != null)
        {
            tvInteractable.enabled = enabled;
        }

        if (tvController != null)
        {
            tvController.enabled = enabled;
        }
    }

    private void ShowDialogMessage()
    {
        if (dialogText == null)
        {
            return;
        }

        dialogText.text = message;

        if (!dialogText.gameObject.activeSelf)
        {
            dialogText.gameObject.SetActive(true);
        }

        Transform parent = dialogText.transform.parent;
        if (parent != null && !parent.gameObject.activeSelf)
        {
            parent.gameObject.SetActive(true);
        }

        Canvas canvas = dialogText.canvas;
        if (canvas != null && !canvas.gameObject.activeSelf)
        {
            canvas.gameObject.SetActive(true);
        }
    }

    private void HideDialogMessage()
    {
        if (dialogText == null)
        {
            return;
        }

        if (dialogText.gameObject.activeSelf)
        {
            dialogText.gameObject.SetActive(false);
        }

        Transform parent = dialogText.transform.parent;
        if (parent != null && parent.gameObject.activeSelf)
        {
            parent.gameObject.SetActive(false);
        }
    }
}
