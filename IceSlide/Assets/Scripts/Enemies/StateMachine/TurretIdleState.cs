using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretIdleState : State
{
    TurretStateMachine turretStateMachine;
    public TurretIdleState(StateMachine stateMachine) : base(stateMachine)
    {
        turretStateMachine = (TurretStateMachine)stateMachine;
    }

    public override void OnUpdateState()
    {
        turretStateMachine.Turret.Eje.Rotate(turretStateMachine.Turret.Eje.forward, Time.deltaTime * turretStateMachine.Turret.RotationSpeed);
    }

}
