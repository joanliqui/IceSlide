using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected State _cntState;

    public void SetState(State state)
    {
        _cntState = state;
        _cntState?.OnEnterState();
    }

    public State GetCntState()
    {
        return _cntState;
    }


}
