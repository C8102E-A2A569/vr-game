using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private void Start()
    {
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
        SetDeviceSimulatorEnabled(false);
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
        SetDeviceSimulatorEnabled(true);
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
    }

    private void SetDeviceSimulatorEnabled(bool enabled)
    {
        if (xrDeviceSimulatorRoot == null)
        {
            xrDeviceSimulatorRoot = GameObject.Find("XR Device Simulator");
        }

        MonoBehaviour[] behaviours = xrDeviceSimulatorRoot != null
            ? xrDeviceSimulatorRoot.GetComponentsInChildren<MonoBehaviour>(true)
            : FindObjectsOfType<MonoBehaviour>(true);

        for (int i = 0; i < behaviours.Length; i++)
        {
            MonoBehaviour behaviour = behaviours[i];
            if (behaviour == null)
            {
                continue;
            }

            if (behaviour.GetType().Name == "XRDeviceSimulator")
            {
                behaviour.enabled = enabled;
            }
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
}
