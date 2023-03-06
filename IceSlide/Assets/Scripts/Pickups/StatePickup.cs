using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePickup : MonoBehaviour, IPickeable
{
    Collider2D col;
    SpriteRenderer sr;
    PlayerAttack1 playerAttack;
    [SerializeField] StateType stateType;
    Color stateColor;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack1>();

        StateDictionarySO.stateColorDisctionary.TryGetValue(stateType, out stateColor);
        sr.color = stateColor;
    }

    public void Pick()
    {
        playerAttack.SetStateType(stateType);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Pick();
            Deactivate();
        }
    }

    private void Deactivate()
    {
        sr.enabled = false;
        col.enabled = false;
    }
}
