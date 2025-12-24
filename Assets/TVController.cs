using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TVController : MonoBehaviour
{
    public Material tvMaterial; // Материал телевизора
    public Camera playerCamera; // Камера персонажа
    public float interactDistance = 3f; // Максимальная дистанция для взаимодействия
    [SerializeField] private Renderer screenRenderer;

    private bool isOn = false;
    private Material instanceMaterial;
    private Material offMaterial;
    private XRBaseInteractable xrInteractable;

    void Start()
    {
        // Создаем уникальный материал для этого объекта, чтобы менять параметры локально
        if (screenRenderer == null)
        {
            screenRenderer = GetComponent<Renderer>();
        }

        if (screenRenderer != null)
        {
            Material materialToUse = tvMaterial != null ? tvMaterial : screenRenderer.sharedMaterial;
            instanceMaterial = materialToUse != null ? new Material(materialToUse) : null;
            offMaterial = CreateBlackMaterial(materialToUse);
            ApplyState(false);
        }

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
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.G))
                {
                    ToggleTv();
                }
            }
        }
    }

    private void Awake()
    {
        xrInteractable = GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        if (xrInteractable != null)
        {
            xrInteractable.selectEntered.AddListener(HandleSelectEntered);
        }
    }

    private void OnDisable()
    {
        if (xrInteractable != null)
        {
            xrInteractable.selectEntered.RemoveListener(HandleSelectEntered);
        }
    }

    private void HandleSelectEntered(SelectEnterEventArgs args)
    {
        ToggleTv();
    }

    private void ToggleTv()
    {
        isOn = !isOn;
        ApplyState(isOn);
    }

    private void ApplyState(bool enabled)
    {
        if (screenRenderer == null)
        {
            return;
        }

        if (enabled)
        {
            if (instanceMaterial != null)
            {
                SetOnState(instanceMaterial, true);
                screenRenderer.material = instanceMaterial;
            }
        }
        else
        {
            if (offMaterial != null)
            {
                screenRenderer.material = offMaterial;
            }
            if (instanceMaterial != null)
            {
                SetOnState(instanceMaterial, false);
            }
        }
    }

    private static void SetOnState(Material material, bool enabled)
    {
        if (material == null)
        {
            return;
        }

        float value = enabled ? 1f : 0f;
        if (material.HasProperty("_IsOn"))
        {
            material.SetFloat("_IsOn", value);
        }
        if (material.HasProperty("_ISON"))
        {
            material.SetFloat("_ISON", value);
        }
        if (material.HasProperty("_IsON"))
        {
            material.SetFloat("_IsON", value);
        }
        if (enabled)
        {
            material.EnableKeyword("_ISON");
        }
        else
        {
            material.DisableKeyword("_ISON");
        }
    }

    private static Material CreateBlackMaterial(Material reference)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }
        if (shader == null)
        {
            shader = reference != null ? reference.shader : Shader.Find("Universal Render Pipeline/Lit");
        }
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        if (shader == null)
        {
            return null;
        }

        Material material = new Material(shader);
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", Color.black);
        }
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", Color.black);
        }
        if (material.HasProperty("_EmissionColor"))
        {
            material.SetColor("_EmissionColor", Color.black);
        }
        return material;
    }

}
