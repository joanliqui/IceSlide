using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudioHandler : MonoBehaviour
{
    AudioSource source;
    [SerializeField] AudioClip damageClip;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    

    public void PlayDamageSound()
    {
        if (source.clip != damageClip)
            source.clip = damageClip;

        source.Play();
        source.pitch += 0.3f;
    }

    public void ChangePitch(float pitch)
    {
        pitch = Mathf.Clamp(pitch, 0, 3);
        source.pitch = pitch;
    }
}
