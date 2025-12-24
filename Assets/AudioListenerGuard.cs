using UnityEngine;

public class AudioListenerGuard : MonoBehaviour
{
    [SerializeField] private AudioListener preferredListener;

    private void Awake()
    {
        if (preferredListener == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                preferredListener = mainCamera.GetComponent<AudioListener>();
            }
        }

        DisableExtraListeners();
    }

    private void OnEnable()
    {
        DisableExtraListeners();
    }

    private void DisableExtraListeners()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>(true);
        AudioListener listenerToKeep = preferredListener;

        if (listenerToKeep == null)
        {
            for (int i = 0; i < listeners.Length; i++)
            {
                if (listeners[i].enabled)
                {
                    listenerToKeep = listeners[i];
                    break;
                }
            }
        }

        for (int i = 0; i < listeners.Length; i++)
        {
            AudioListener listener = listeners[i];
            if (listener == null)
            {
                continue;
            }

            if (listenerToKeep != null && listener == listenerToKeep)
            {
                listener.enabled = true;
                continue;
            }

            listener.enabled = false;
        }
    }
}
