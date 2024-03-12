using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CumState_Pair : IState
{
    public void OnEnterState(object action)
    {
        var player = (PlayerActor_Sexy)action;
    }

    public void OnStayState(object action)
    {
        var player = (PlayerActor_Sexy)action;
    }

    public void OnExitState(object action)
    {
        var player = (PlayerActor_Sexy)action;
    }
}
