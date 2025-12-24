using UnityEngine;

public class SkyboxGradient : MonoBehaviour
{
    [SerializeField] private Color topColor = new Color(0.9f, 0.66f, 0.82f, 1f);
    [SerializeField] private Color bottomColor = new Color(0.16f, 0.2f, 0.32f, 1f);
    [SerializeField] private int textureHeight = 256;
    [SerializeField] private float exposure = 1f;
    [SerializeField] private bool applyOnStart = true;

    private Material runtimeMaterial;

    private void Start()
    {
        if (applyOnStart)
        {
            Apply();
        }
    }

    public void Apply()
    {
        Shader shader = Shader.Find("Skybox/6 Sided");
        if (shader == null)
        {
            shader = Shader.Find("Skybox/Procedural");
        }

        if (shader == null)
        {
            return;
        }

        Texture2D gradientTexture = BuildGradientTexture();

        runtimeMaterial = new Material(shader);

        if (shader.name.Contains("6 Sided"))
        {
            runtimeMaterial.SetTexture("_FrontTex", gradientTexture);
            runtimeMaterial.SetTexture("_BackTex", gradientTexture);
            runtimeMaterial.SetTexture("_LeftTex", gradientTexture);
            runtimeMaterial.SetTexture("_RightTex", gradientTexture);
            runtimeMaterial.SetTexture("_UpTex", gradientTexture);
            runtimeMaterial.SetTexture("_DownTex", gradientTexture);
        }
        else
        {
            if (runtimeMaterial.HasProperty("_SkyTint"))
            {
                runtimeMaterial.SetColor("_SkyTint", topColor);
            }
            if (runtimeMaterial.HasProperty("_GroundColor"))
            {
                runtimeMaterial.SetColor("_GroundColor", bottomColor);
            }
        }

        if (runtimeMaterial.HasProperty("_Exposure"))
        {
            runtimeMaterial.SetFloat("_Exposure", exposure);
        }

        RenderSettings.skybox = runtimeMaterial;
        DynamicGI.UpdateEnvironment();
    }

    private Texture2D BuildGradientTexture()
    {
        int height = Mathf.Max(2, textureHeight);
        Texture2D texture = new Texture2D(2, height, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < height; y++)
        {
            float t = (float)y / (height - 1);
            Color color = Color.Lerp(bottomColor, topColor, t);
            texture.SetPixel(0, y, color);
            texture.SetPixel(1, y, color);
        }

        texture.Apply();
        return texture;
    }
}
