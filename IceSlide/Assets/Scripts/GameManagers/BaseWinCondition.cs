using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseWinCondition : MonoBehaviour
{
    public abstract void StepForWin();

    public abstract void CheckWinCondition();

    protected abstract void Win();
}
