using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    protected StateMachine stateMachine;

    protected State(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void OnEnterState()
    {

    }

    public virtual void OnUpdateState()
    {

    }

    public virtual void OnExitState()
    {

    }
}
