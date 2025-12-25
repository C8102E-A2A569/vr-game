using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private PlayerFirstPerson PlayerFirstPerson;
    [SerializeField] private FootstepOnMove footstepOnMove;
    [SerializeField] private GameObject xrDeviceSimulatorRoot;
    [SerializeField] private bool useTimeScalePause = false;
    [SerializeField] private float pausedTimeScale = 0f;
    [SerializeField] private float resumeTimeScale = 1f;

    private bool isPaused;
    private Canvas pauseCanvas;

    private void Start()
    {
        EnsureEventSystem();
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        ResolveMovementReferences();
        SetupPauseCanvas();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }

        if (useTimeScalePause)
        {
            Time.timeScale = pausedTimeScale;
        }
        SetMovementEnabled(false);
        Physics.SyncTransforms();
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        if (useTimeScalePause)
        {
            Time.timeScale = resumeTimeScale;
        }
        Physics.SyncTransforms();
        SetMovementEnabled(true);
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
        StartCoroutine(EnsureCursorLock());
    }

    public void QuitToMainMenu()
    {
        if (useTimeScalePause)
        {
            Time.timeScale = resumeTimeScale;
        }
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    private void ResolveMovementReferences()
    {
        if (PlayerFirstPerson == null)
        {
            PlayerFirstPerson[] players = FindObjectsOfType<PlayerFirstPerson>(true);
            if (players.Length > 0)
            {
                PlayerFirstPerson = players[0];
            }
        }

        if (footstepOnMove == null)
        {
            FootstepOnMove[] footsteps = FindObjectsOfType<FootstepOnMove>(true);
            if (footsteps.Length > 0)
            {
                footstepOnMove = footsteps[0];
            }
        }

        if (xrDeviceSimulatorRoot == null)
        {
            xrDeviceSimulatorRoot = GameObject.Find("XR Device Simulator");
        }
    }

    private void SetMovementEnabled(bool enabled)
    {
        ResolveMovementReferences();

        PlayerFirstPerson[] players = FindObjectsOfType<PlayerFirstPerson>(true);
        for (int i = 0; i < players.Length; i++)
        {
            PlayerFirstPerson player = players[i];
            if (player == null)
            {
                continue;
            }

            player.SetControlEnabled(enabled);
            player.enabled = enabled;

            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = enabled;
            }
        }

        FootstepOnMove[] footsteps = FindObjectsOfType<FootstepOnMove>(true);
        for (int i = 0; i < footsteps.Length; i++)
        {
            FootstepOnMove step = footsteps[i];
            if (step != null)
            {
                step.enabled = enabled;
            }
        }

        LocomotionProvider[] locomotionProviders = FindObjectsOfType<LocomotionProvider>(true);
        for (int i = 0; i < locomotionProviders.Length; i++)
        {
            LocomotionProvider provider = locomotionProviders[i];
            if (provider != null)
            {
                provider.enabled = enabled;
            }
        }
    }

    private void SetDeviceSimulatorEnabled(bool enabled)
    {
        return;
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

    private void SetupPauseCanvas()
    {
        if (pauseMenuUI == null || pauseCanvas != null)
        {
            return;
        }

        Camera mainCamera = ResolveMainCamera();
        if (mainCamera == null)
        {
            return;
        }

        Canvas parentCanvas = pauseMenuUI.GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            pauseCanvas = pauseMenuUI.AddComponent<Canvas>();
            pauseMenuUI.AddComponent<CanvasScaler>();
            pauseMenuUI.AddComponent<GraphicRaycaster>();
        }
        else if (parentCanvas.transform != pauseMenuUI.transform && parentCanvas.transform.childCount > 1)
        {
            GameObject pauseCanvasObject = new GameObject("PauseMenuCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            pauseCanvasObject.layer = pauseMenuUI.layer;
            pauseCanvas = pauseCanvasObject.GetComponent<Canvas>();
            pauseCanvasObject.transform.SetParent(mainCamera.transform, false);
            pauseMenuUI.transform.SetParent(pauseCanvasObject.transform, false);
        }
        else
        {
            pauseCanvas = parentCanvas;
        }

        pauseCanvas.renderMode = RenderMode.WorldSpace;
        pauseCanvas.worldCamera = mainCamera;

        RectTransform rectTransform = pauseCanvas.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.SetParent(mainCamera.transform, false);
            rectTransform.sizeDelta = new Vector2(900f, 600f);
            rectTransform.localPosition = new Vector3(0f, 0f, 1.6f);
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one * 0.0015f;
        }

        if (pauseCanvas.GetComponent<TrackedDeviceGraphicRaycaster>() == null)
        {
            pauseCanvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
        }

        int uiLayer = LayerMask.NameToLayer("UI");
        if (uiLayer >= 0)
        {
            SetLayerRecursively(pauseCanvas.gameObject, uiLayer);
        }
    }

    private System.Collections.IEnumerator EnsureCursorLock()
    {
        yield return null;
        if (!isPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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

    private void SetLayerRecursively(GameObject target, int layer)
    {
        if (target == null)
        {
            return;
        }

        target.layer = layer;
        Transform[] children = target.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            Transform child = children[i];
            if (child != null)
            {
                child.gameObject.layer = layer;
            }
        }
    }
}
