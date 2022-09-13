using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingHandler : MonoBehaviour
{
    Volume volume;
    ChromaticAberration cha;
    Vignette vignette;

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

        volume.enabled = true;
        vignette.active = true;
        cha.active = true;
        ResetValues();
    }

    public void SetChromaticAberrationValue(float a)
    {
        cha.intensity.Override(Mathf.Clamp(a, 0.0f, chaMaxIntensity));
    }

    public void SetVignetteValue(float a)
    {
        vignette.intensity.Override(Mathf.Clamp(a, 0.0f, vigneteMaxIntesity));
    }

    public void ResetValues()
    {
        cha.intensity.Override(0.0f);
        vignette.intensity.Override(0.0f);
    }
    public void HandlePostProcessing(bool state)
    {
        volume.enabled = state;
    }
   
}
