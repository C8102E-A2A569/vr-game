using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class MainMenuVrBootstrap : MonoBehaviour
{
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private GameObject xrOriginPrefab;
    [SerializeField] private GameObject xrInteractionSetupPrefab;
    [SerializeField] private GameObject xrDeviceSimulatorPrefab;
    [SerializeField] private float canvasDistance = 1.6f;
    [SerializeField] private float canvasScale = 0.0015f;
    [SerializeField] private Vector2 canvasSize = new Vector2(900f, 600f);

    private void Start()
    {
        EnsureEventSystem();
        EnsureXrRig();
        StartCoroutine(AttachWhenReady());
    }

    private System.Collections.IEnumerator AttachWhenReady()
    {
        yield return null;
        AttachCanvasToCamera();
    }

    private void EnsureXrRig()
    {
        if (GameObject.Find("XR Origin (XR Rig)") == null && xrOriginPrefab != null)
        {
            Instantiate(xrOriginPrefab);
        }

        if (GameObject.Find("XR Interaction Setup") == null && xrInteractionSetupPrefab != null)
        {
            Instantiate(xrInteractionSetupPrefab);
        }

        if (GameObject.Find("XR Device Simulator") == null && xrDeviceSimulatorPrefab != null)
        {
            Instantiate(xrDeviceSimulatorPrefab);
        }
    }

    private void AttachCanvasToCamera()
    {
        if (menuCanvas == null)
        {
            return;
        }

        Camera mainCamera = ResolveMainCamera();
        if (mainCamera == null)
        {
            return;
        }

        menuCanvas.renderMode = RenderMode.WorldSpace;
        menuCanvas.worldCamera = mainCamera;

        RectTransform rectTransform = menuCanvas.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.SetParent(mainCamera.transform, false);
            rectTransform.sizeDelta = canvasSize;
            rectTransform.localPosition = new Vector3(0f, 0f, canvasDistance);
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one * canvasScale;
        }

        if (menuCanvas.GetComponent<TrackedDeviceGraphicRaycaster>() == null)
        {
            menuCanvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
        }
    }

    private void EnsureEventSystem()
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>(true);
        EventSystem eventSystem = null;
        for (int i = 0; i < eventSystems.Length; i++)
        {
            EventSystem candidate = eventSystems[i];
            if (candidate != null && candidate.gameObject.activeInHierarchy)
            {
                eventSystem = candidate;
                break;
            }
        }

        if (eventSystem == null && eventSystems.Length > 0)
        {
            eventSystem = eventSystems[0];
        }

        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(XRUIInputModule));
            eventSystem = eventSystemObject.GetComponent<EventSystem>();
        }

        if (!eventSystem.gameObject.activeInHierarchy)
        {
            eventSystem.gameObject.SetActive(true);
        }

        for (int i = 0; i < eventSystems.Length; i++)
        {
            EventSystem otherSystem = eventSystems[i];
            if (otherSystem != null && otherSystem != eventSystem)
            {
                otherSystem.gameObject.SetActive(false);
            }
        }

        XRUIInputModule xrInputModule = eventSystem.GetComponent<XRUIInputModule>();
        if (xrInputModule == null)
        {
            xrInputModule = eventSystem.gameObject.AddComponent<XRUIInputModule>();
        }

        xrInputModule.enableXRInput = true;
        xrInputModule.enableBuiltinActionsAsFallback = true;

        StandaloneInputModule standaloneInputModule = eventSystem.GetComponent<StandaloneInputModule>();
        if (standaloneInputModule != null)
        {
            standaloneInputModule.enabled = false;
        }

        InputSystemUIInputModule[] inputModules = eventSystem.GetComponents<InputSystemUIInputModule>();
        for (int i = 0; i < inputModules.Length; i++)
        {
            InputSystemUIInputModule module = inputModules[i];
            if (module != xrInputModule)
            {
                module.enabled = false;
            }
        }
    }

    private Camera ResolveMainCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            return mainCamera;
        }

        Camera[] cameras = FindObjectsOfType<Camera>(true);
        for (int i = 0; i < cameras.Length; i++)
        {
            Camera candidate = cameras[i];
            if (candidate != null && candidate.enabled)
            {
                return candidate;
            }
        }

        return null;
    }
}
