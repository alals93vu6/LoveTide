using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPinchState_Chests : IState
{
    public void OnEnterState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
        chests.stateAnimator[0] = "被玩弄";
        chests.stateAnimator[1] = "被玩弄_慢";
        chests.stateAnimator[2] = "被玩弄_中";
        chests.stateAnimator[3] = "被玩弄_快";
        chests.SwitchAnimator();
        chests.stimulation = true;
    }

    public void OnStayState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
    }

    public void OnExitState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
    }
}