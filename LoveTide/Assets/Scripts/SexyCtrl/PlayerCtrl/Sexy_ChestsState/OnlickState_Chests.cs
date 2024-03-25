using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlickState_Chests : IState
{
    public void OnEnterState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
        chests.stateAnimator[0] = "被舔";
        chests.stateAnimator[1] = "被舔_慢";
        chests.stateAnimator[2] = "被舔_中";
        chests.stateAnimator[3] = "被舔_快";
        chests.SwitchAnimator();
        chests.haveMouth = true;
        chests.stimulation = true;
    }

    public void OnStayState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
    }

    public void OnExitState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
        chests.haveMouth = false;
    }
}