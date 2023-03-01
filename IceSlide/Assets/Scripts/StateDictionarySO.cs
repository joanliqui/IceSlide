using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StateDictionarySO
{
    public static Dictionary<StateType, Color> stateColorDisctionary = new Dictionary<StateType, Color>()
    {
        {StateType.Neutral, Color.red },
        {StateType.Black, Color.black },
        {StateType.White, Color.white }
    };
}
