using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClampedState_Chests : IState
{
    public void OnEnterState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
        chests.stateAnimator[0] = "被夾住";
        chests.stateAnimator[1] = "被夾住_慢";
        chests.stateAnimator[2] = "被夾住_中";
        chests.stateAnimator[3] = "被夾住_快";
        chests.SwitchAnimator();
        chests.stimulation = false;
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