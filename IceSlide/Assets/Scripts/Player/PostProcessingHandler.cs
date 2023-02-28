using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingHandler : MonoBehaviour
{
    Volume volume;
    ChromaticAberration cha;
    Vignette vignette;
    ColorAdjustments colorAdjustments;

    [Header("PostPro Max Values")]
    [Range(0.0f, 1.0f)]
    [SerializeField] float chaMaxIntensity = 0.6f;
    [Range(0.0f, 1.0f)]
    [SerializeField] float vigneteMaxIntesity = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet<ChromaticAberration>(out cha);
        volume.profile.TryGet<Vignette>(out vignette);
        volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);

        volume.enabled = true;
        vignette.active = true;
        cha.active = true;
        colorAdjustments.active = true;
        ResetValuesForDash();
    }

    public void SetChromaticAberrationValue(float a)
    {
        if(cha)
            cha.intensity.Override(Mathf.Clamp(a, 0.0f, chaMaxIntensity));
    }

    public void SetVignetteValue(float a)
    {
        if(vignette)
            vignette.intensity.Override(Mathf.Clamp(a, 0.0f, vigneteMaxIntesity));
    }

    public void SetSaturationValue(int a)
    {
        if(colorAdjustments)
            colorAdjustments.saturation.Override(Mathf.Clamp(a, -100, 0));
    }

    public void ResetValuesForDash()
    {
        if(cha && vignette)
        {
            cha.intensity.Override(0.0f);
            vignette.intensity.Override(0.0f);
        }
    }
    public void HandlePostProcessing(bool state)
    {
        if(volume)
            volume.enabled = state;
    }
   
}
