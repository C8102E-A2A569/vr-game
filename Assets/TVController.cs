using UnityEngine;

public class TVController : MonoBehaviour
{
    public Material tvMaterial; // Материал телевизора
    public Camera playerCamera; // Камера персонажа
    public float interactDistance = 3f; // Максимальная дистанция для взаимодействия

    private bool isOn = false;
    private Material instanceMaterial;

    void Start()
    {
        // Создаем уникальный материал для этого объекта, чтобы менять параметры локально
        instanceMaterial = new Material(tvMaterial);
        GetComponent<Renderer>().material = instanceMaterial;
    }

    void Update()
    {
        // Пускаем луч из центра камеры вперед
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // Проверяем, попал ли луч в этот объект (телевизор) на расстоянии interactDistance
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                // Если нажали клавишу E — переключаем состояние телевизора
                if (Input.GetKeyDown(KeyCode.E))
                {
                    isOn = !isOn;
                    Debug.Log("TV toggled. IsOn = " + isOn);
                    instanceMaterial.SetFloat("_IsOn", isOn ? 1f : 0f);
                }
            }
        }
    }
}
