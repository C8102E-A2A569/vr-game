using UnityEngine;

public class ItemInteractor : MonoBehaviour
{
    public float interactDistance = 6f;
    public LayerMask interactLayer; // Îעלועü סכמי הכÿ ןנוהלועמג
    public Camera playerCamera;
    private InventoryUI inventory;

    void Start()
    {
        inventory = FindObjectOfType<InventoryUI>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
            {
                PickupItem item = hit.collider.GetComponent<PickupItem>();
                if (item != null)
                {
                    bool added = inventory.AddItem(item.itemIcon);
                    if (added) Destroy(item.gameObject);
                }
            }
        }
    }
}
