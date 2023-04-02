using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePickup : MonoBehaviour, IPickeable
{
    Collider2D col;
    SpriteRenderer sr;
    PlayerAttack1 playerAttack;
    AudioSource source;

    [SerializeField] StateType stateType;
    Color stateColor;

    [Header("Distortion Variables")]
    [SerializeField] SpriteRenderer srWithMaterial;
    [SerializeField] float deactivationSpeed = 0.05f;

    private const string DISTORTIONSCALE = "_distortionScale";
    private bool lerpDistortion = false;
    private Material distortionMaterial;
    float cntDistortionAmount;

    public delegate void Picked();
    private event Picked onPicked;
    private void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack1>();
        source = GetComponent<AudioSource>();

        StateDictionarySO.stateColorDisctionary.TryGetValue(stateType, out stateColor);
        sr.color = stateColor;
        
        if(srWithMaterial)
            distortionMaterial = srWithMaterial.material;

        if (distortionMaterial)
        {
            cntDistortionAmount = distortionMaterial.GetFloat(DISTORTIONSCALE);
        }

        onPicked += Pick;
        onPicked += Deactivate;
        onPicked += DeactivateDistortion;
        onPicked += SoundOnPick;
    }

    public void Pick()
    {
        playerAttack.SetStateType(stateType);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            onPicked?.Invoke();
        }
    }

    private void Deactivate()
    {
        sr.enabled = false;
        col.enabled = false;
    }

    private void DeactivateDistortion()
    {
        if (!distortionMaterial) return;
        lerpDistortion = true;
    }

    private void Update()
    {
        if (lerpDistortion)
        {
            if(cntDistortionAmount > 0.0f)
            {
                cntDistortionAmount -= Time.deltaTime * deactivationSpeed;
                distortionMaterial.SetFloat(DISTORTIONSCALE, cntDistortionAmount);
            }
            else
            {
                srWithMaterial.enabled = false;
                lerpDistortion = false;
            }
        }
    }

    private void SoundOnPick()
    {
        if (!source) return;

        source.Play();
    }
}
