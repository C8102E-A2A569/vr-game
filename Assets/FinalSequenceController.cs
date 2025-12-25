using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalSequenceController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TVController tvController;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform spinPivot;
    [SerializeField] private Transform[] spinRoots;
    [SerializeField] private Behaviour[] disableBehaviours;
    [SerializeField] private GameObject[] disableObjects;
    [SerializeField] private GameObject[] hideOnSequenceStart;

    [Header("Audio")]
    [SerializeField] private AudioClip lowRumbleClip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float rumbleVolume = 0.7f;
    [SerializeField] private float rumbleFadeIn = 1.5f;
    [SerializeField] private float rumbleFadeOut = 1f;

    [Header("Spin")]
    [SerializeField] private float spinDuration = 11.5f;
    [SerializeField] private float maxSpinSpeed = 220f;
    [SerializeField] private AnimationCurve spinSpeedCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.2f, 0.6f),
        new Keyframe(1f, 1f)
    );

    [Header("Post FX")]
    [SerializeField] private float chromaticIntensity = 1f;
    [SerializeField] private float vignetteIntensity = 0.45f;
    [SerializeField] private float lensDistortionIntensity = -0.45f;

    [Header("Flash")]
    [SerializeField] private float flashFadeIn = 1.75f;
    [SerializeField] private float flashHold = 1.25f;
    [SerializeField] private float flashFadeOut = 2.25f;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private Volume volume;
    private ChromaticAberration chromatic;
    private Vignette vignette;
    private LensDistortion lensDistortion;
    private Image flashImage;
    private bool hasStarted;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (spinPivot == null && targetCamera != null)
        {
            spinPivot = targetCamera.transform;
        }

        if ((spinRoots == null || spinRoots.Length == 0) && transform != null)
        {
            spinRoots = ResolveSpinRoots();
        }

        spinRoots = AppendNamedRoot(spinRoots, "комната");
        spinRoots = AppendNamedRoot(spinRoots, "second_room");
        spinRoots = AppendNamedRoot(spinRoots, "Пол");

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        EnsurePostProcessing();
        EnsureOverlay();
        ConfigureAudio();
    }

    public void StartFinalSequence()
    {
        if (hasStarted)
        {
            return;
        }

        hasStarted = true;
        HideSequenceObjects();
        DisablePlayerControls();
        StartCoroutine(SequenceRoutine());
    }

    private void DisablePlayerControls()
    {
        if (disableBehaviours != null)
        {
            for (int i = 0; i < disableBehaviours.Length; i++)
            {
                Behaviour behaviour = disableBehaviours[i];
                if (behaviour != null)
                {
                    behaviour.enabled = false;
                }
            }
        }

        if (disableObjects != null)
        {
            for (int i = 0; i < disableObjects.Length; i++)
            {
                GameObject target = disableObjects[i];
                if (target != null)
                {
                    target.SetActive(false);
                }
            }
        }
    }

    private IEnumerator SequenceRoutine()
    {
        float rumbleTimer = 0f;
        if (audioSource != null && lowRumbleClip != null)
        {
            audioSource.volume = 0f;
            audioSource.loop = true;
            audioSource.clip = lowRumbleClip;
            audioSource.Play();
        }

        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            float deltaTime = Time.deltaTime;
            float normalized = Mathf.Clamp01(elapsed / spinDuration);
            float speed = maxSpinSpeed * spinSpeedCurve.Evaluate(normalized);
            ApplySpin(speed * deltaTime);
            UpdatePostProcessing(normalized);

            if (audioSource != null && lowRumbleClip != null && rumbleFadeIn > 0f)
            {
                rumbleTimer += deltaTime;
                audioSource.volume = Mathf.Clamp01(rumbleTimer / rumbleFadeIn) * rumbleVolume;
            }

            elapsed += deltaTime;
            yield return null;
        }

        yield return FlashRoutine();

        if (audioSource != null && lowRumbleClip != null)
        {
            StartCoroutine(FadeOutAudio());
        }

        UpdatePostProcessing(0f);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void HideSequenceObjects()
    {
        if (hideOnSequenceStart == null)
        {
            return;
        }

        for (int i = 0; i < hideOnSequenceStart.Length; i++)
        {
            GameObject target = hideOnSequenceStart[i];
            if (target != null)
            {
                target.SetActive(false);
            }
        }
    }

    private IEnumerator FlashRoutine()
    {
        if (flashImage == null)
        {
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < flashFadeIn)
        {
            float alpha = flashFadeIn <= 0f ? 1f : elapsed / flashFadeIn;
            SetImageAlpha(flashImage, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetImageAlpha(flashImage, 1f);
        if (flashHold > 0f)
        {
            yield return new WaitForSeconds(flashHold);
        }

        SetImageAlpha(flashImage, 1f);
    }

    private IEnumerator FadeOutAudio()
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;
        while (elapsed < rumbleFadeOut)
        {
            float t = rumbleFadeOut <= 0f ? 1f : elapsed / rumbleFadeOut;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    private void ApplySpin(float deltaDegrees)
    {
        if (spinRoots == null || spinPivot == null)
        {
            return;
        }

        for (int i = 0; i < spinRoots.Length; i++)
        {
            Transform root = spinRoots[i];
            if (root == null)
            {
                continue;
            }

            root.RotateAround(spinPivot.position, Vector3.up, deltaDegrees);
        }
    }

    private void EnsurePostProcessing()
    {
        if (targetCamera != null)
        {
            UniversalAdditionalCameraData cameraData = targetCamera.GetUniversalAdditionalCameraData();
            if (cameraData != null)
            {
                cameraData.renderPostProcessing = true;
            }
        }

        if (volume != null)
        {
            return;
        }

        GameObject volumeObject = new GameObject("FinalSequenceVolume");
        volumeObject.transform.SetParent(transform, false);
        volumeObject.layer = 0;

        volume = volumeObject.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 100f;
        volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();

        chromatic = volume.profile.Add<ChromaticAberration>(true);
        vignette = volume.profile.Add<Vignette>(true);
        lensDistortion = volume.profile.Add<LensDistortion>(true);

        chromatic.intensity.Override(0f);
        vignette.intensity.Override(0f);
        lensDistortion.intensity.Override(0f);
    }

    private void UpdatePostProcessing(float normalized)
    {
        if (chromatic != null)
        {
            chromatic.intensity.Override(chromaticIntensity * normalized);
        }
        if (vignette != null)
        {
            vignette.intensity.Override(vignetteIntensity * normalized);
        }
        if (lensDistortion != null)
        {
            lensDistortion.intensity.Override(lensDistortionIntensity * normalized);
        }
    }

    private void EnsureOverlay()
    {
        if (flashImage != null)
        {
            return;
        }

        GameObject canvasObject = new GameObject("FinalSequenceOverlay");
        canvasObject.transform.SetParent(transform, false);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject flashObject = new GameObject("Flash");
        flashObject.transform.SetParent(canvasObject.transform, false);
        flashImage = flashObject.AddComponent<Image>();
        flashImage.raycastTarget = false;
        flashImage.color = new Color(1f, 1f, 1f, 0f);
        StretchToFullScreen(flashObject.GetComponent<RectTransform>());
    }

    private void ConfigureAudio()
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.playOnAwake = false;
        audioSource.loop = true;
    }

    private static void StretchToFullScreen(RectTransform rectTransform)
    {
        if (rectTransform == null)
        {
            return;
        }

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
    }

    private static void SetImageAlpha(Graphic graphic, float alpha)
    {
        if (graphic == null)
        {
            return;
        }

        Color color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }

    private Transform[] ResolveSpinRoots()
    {
        List<Transform> roots = new List<Transform>();
        AddRootByName(roots, "final_roomm");
        AddRootByName(roots, "комната");
        return roots.ToArray();
    }

    private static void AddRootByName(List<Transform> roots, string rootName)
    {
        if (string.IsNullOrEmpty(rootName))
        {
            return;
        }

        GameObject rootObject = GameObject.Find(rootName);
        if (rootObject != null)
        {
            roots.Add(rootObject.transform);
        }
    }

    private static Transform[] AppendNamedRoot(Transform[] roots, string rootName)
    {
        if (string.IsNullOrEmpty(rootName))
        {
            return roots;
        }

        GameObject rootObject = GameObject.Find(rootName);
        if (rootObject == null)
        {
            return roots;
        }

        Transform rootTransform = rootObject.transform;
        if (rootTransform == null)
        {
            return roots;
        }

        if (roots == null)
        {
            return new[] { rootTransform };
        }

        for (int i = 0; i < roots.Length; i++)
        {
            if (roots[i] == rootTransform)
            {
                return roots;
            }
        }

        List<Transform> merged = new List<Transform>(roots) { rootTransform };
        return merged.ToArray();
    }
}
