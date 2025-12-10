using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DoorInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 3f;
    public InventoryUI inventory;
    public Animator doorAnimator;
    public string keySpriteName = "Key";

    [Header("Звук двери")]
    public AudioSource audioSource;
    public AudioClip openDoorSound;

    [Header("Конец игры")]
    public GameObject endGamePanel; // Панель "Игра пройдена"
    public float delayBeforeMainMenu = 10f;
    public string mainMenuSceneName = "MainMenu"; // Название сцены главного меню

    private bool gameEnded = false;

    void Update()
    {
        if (gameEnded) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    int keySlotIndex = FindKeySlot();
                    if (keySlotIndex != -1)
                    {
                        OpenDoor();
                        RemoveKeyFromInventory(keySlotIndex);
                    }
                    else
                    {
                        Debug.Log("Нужен ключ!");
                    }
                }
            }
        }
    }

    int FindKeySlot()
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            var slot = inventory.slots[i];
            if (slot.currentItem != null && slot.currentItem.name == keySpriteName && slot.count > 0)
                return i;
        }
        return -1;
    }

    void RemoveKeyFromInventory(int slotIndex)
    {
        var slot = inventory.slots[slotIndex];
        slot.count--;
        if (slot.count <= 0)
        {
            slot.count = 0;
            slot.currentItem = null;
            slot.icon.sprite = null;
            slot.icon.enabled = false;
            slot.countText.text = "";
        }
        else
        {
            slot.countText.text = slot.count.ToString();
        }
    }

    void OpenDoor()
    {
        doorAnimator.SetTrigger("Open");
        Debug.Log("Дверь открывается!");

        if (audioSource != null && openDoorSound != null)
        {
            audioSource.PlayOneShot(openDoorSound);
        }

        EndGame();
    }

    void EndGame()
    {
        gameEnded = true;

        if (endGamePanel != null)
            endGamePanel.SetActive(true);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Invoke(nameof(LoadMainMenu), delayBeforeMainMenu);
    }

    void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
