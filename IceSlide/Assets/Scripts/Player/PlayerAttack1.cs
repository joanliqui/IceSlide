using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class PlayerAttack1 : MonoBehaviour
{
    PlayerMovement1 player;
    PlayerLife life;
    SpriteRenderer sr;

    private StateType stateType = StateType.White;
    private List<StateType> typesList = new List<StateType>();
    private int cntState = 1;
    Color stateColor = Color.white;

    public Color StateColor { get => stateColor; set => stateColor = value; }

    private void Start()
    {
        player = GetComponent<PlayerMovement1>();
        life = GetComponent<PlayerLife>();
        sr = GetComponent<SpriteRenderer>();

        StateDictionarySO.stateColorDisctionary.ToList().ForEach(pair => 
        {
            typesList.Add(pair.Key); 
        });

        typesList.Remove(StateType.Neutral);

        SetStateType(cntState);
    }

    public void SwapStateTypeByInput(float dir)
    {
        if(dir > 0)
        {
            cntState++;
            if(cntState >= typesList.Count)
            {
                cntState = 0;
            }
        }
        else
        {
            cntState--;
            if(cntState < 0)
            {
                cntState = typesList.Count - 1;
            }
        }

        SetStateType(cntState);
    }
    private void SetStateType(int n)
    {
        stateType = typesList[n];
        StateDictionarySO.stateColorDisctionary.TryGetValue(stateType, out stateColor);
        sr.color = stateColor;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (player.IsDashing || player.IsPlusDamage)
        {
            if(collision.transform.TryGetComponent<IDamagable>(out IDamagable d))
            {
                d.Damaged(stateType);
                player.IsDashing = false;
            }
            if(collision.transform.TryGetComponent<IBouncingObject>(out IBouncingObject b))
            {
                Bounce(b.BounceDirection());
            }
        }
        else //El player no es invencible
        {
            if (collision.transform.CompareTag("Enemy"))
            {
                life.PlayerDead();
                player.ArrivedToObjective();
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            if (player.IsDashing)
            {
                if (collision.transform.TryGetComponent<IDamagable>(out IDamagable d))
                {
                    Debug.Log("·asasdasd");
                    d.Damaged(stateType);
                }
            }
        }
    }

    private void Bounce(Vector3 bDir)
    {
        Debug.Log("bOUNCE");
        player.BounceOnDash(bDir);
    }

}
