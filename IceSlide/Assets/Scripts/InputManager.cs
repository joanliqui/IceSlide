using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    private Controls _inputs;

    private Vector2 mousePose;
    [SerializeField] UnityEvent OnDoAttack;

    public Vector2 MousePose { get => mousePose;}

    void Awake()
    {
        _inputs = new Controls();
        _inputs.Player.Aim.performed += onAim;
     //   _inputs.Player.Dash.started += onDash;
       // _inputs.Player.Dash.canceled += onDash;
        _inputs.Player.Attack.started += onAttack;
    }
    private void onAim(InputAction.CallbackContext obj)
    {
        mousePose = obj.ReadValue<Vector2>();
    }
    private void onDash(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }
    private void onAttack(InputAction.CallbackContext obj)
    {
        OnDoAttack?.Invoke();
    }



   

    private void OnEnable()
    {
        _inputs.Enable();
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }
}
