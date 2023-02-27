using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSoundsManager : MonoBehaviour
{
    PlayerMovement1 player;
    [SerializeField] AudioMixerSnapshot normalSnapshot;
    [SerializeField] AudioMixerSnapshot attackSnapshot;
    [SerializeField] AudioMixer mixer;
    void Awake()
    {
        player = GetComponent<PlayerMovement1>();
    }

    private void Start()
    {
        player.onEnterBulletTime += SetAttackSnapshot;
        player.onExitBulletTime += SetNormalSnapshot;
    }


    public void SetNormalSnapshot()
    {
        normalSnapshot.TransitionTo(0f);
    }

    public void SetAttackSnapshot()
    {
        attackSnapshot.TransitionTo(0f);
    }

}
