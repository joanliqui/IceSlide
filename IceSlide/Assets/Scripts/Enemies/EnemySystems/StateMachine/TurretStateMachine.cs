using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretStateMachine : StateMachine
{
    Turret _turret;

    public TurretStateMachine(Turret turret)
    {
        _turret = turret;
    }

    public Turret Turret { get => _turret;}

}
