using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ToggleRenderFeature : MonoBehaviour
{
    public ScriptableRendererData rendererData;  // Koppel in inspector je Renderer Data asset

    private ScriptableRendererFeature invertFeature;
    private ScriptableRendererFeature desaturateFeature;

    void Start()
    {
        if (rendererData == null)
        {
            Debug.LogError("Renderer Data asset is niet toegewezen!");
            return;
        }

        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature.name == "InvertFeature")
                invertFeature = feature;
            else if (feature.name == "DesaturateFeature")
                desaturateFeature = feature;
        }

        if (invertFeature != null)
            invertFeature.SetActive(true);
        if (desaturateFeature != null)
            desaturateFeature.SetActive(false);
    }

    public void ToggleCRT(bool enabled)
    {
        if (invertFeature != null)
            invertFeature.SetActive(enabled);
    }

    public void ToggleDesaturateFeature(bool enabled)
    {
        if (desaturateFeature != null)
            desaturateFeature.SetActive(enabled);
    }
}
