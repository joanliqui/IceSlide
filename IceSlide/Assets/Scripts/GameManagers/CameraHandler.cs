using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraHandler : MonoBehaviour
{
    private static CameraHandler _instance;

    Camera main;
    Camera playerCam;


    [SerializeField] CinemachineVirtualCamera vc;
    CinemachineBasicMultiChannelPerlin shake;
    [SerializeField] float intensity;
    [SerializeField] float time;
    private float shakeTimer;


    public static CameraHandler Instance { get => _instance; }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        main = Camera.main;
        playerCam = main.GetComponentInChildren<Camera>();

        playerCam.orthographicSize = main.orthographicSize;

        shake = vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void CameraShake()
    {
        if (!shake) return;
        shake.m_AmplitudeGain = intensity;
        shakeTimer = time;

    }

    private void Update()
    {
        if(shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            shakeTimer = 0;
            shake.m_AmplitudeGain = 0;
        }
    }
}
