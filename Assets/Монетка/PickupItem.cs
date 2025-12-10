using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Sprite itemIcon; // »конка дл€ отображени€ в UI

    private bool isPlayerNear = false;
    private InventoryUI inventory;

    void Start()
    {
        inventory = FindObjectOfType<InventoryUI>();
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (inventory.AddItem(itemIcon))
            {
                Destroy(gameObject); // удал€ем с карты
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNear = false;
    }
}
