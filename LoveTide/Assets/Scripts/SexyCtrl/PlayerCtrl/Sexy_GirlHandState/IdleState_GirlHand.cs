﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_GirlHand : IState
{
    public void OnEnterState(object action)
    {
        var girlHand = (Sexyctrl_GirlHand)action;
        girlHand.stateAnimator[0] = "G手部:待機";
        girlHand.stateAnimator[1] = "G手部:待機_慢";
        girlHand.stateAnimator[2] = "G手部:待機_中";
        girlHand.stateAnimator[3] = "G手部:待機_快";
        girlHand.SwitchAnimator();
    }

    public void OnStayState(object action)
    {
        var girlHand = (Sexyctrl_GirlHand)action;
    }

    public void OnExitState(object action)
    {
        var girlHand = (Sexyctrl_GirlHand)action;
    }
}