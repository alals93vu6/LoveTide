using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSuckState_Chests : IState
{
    public void OnEnterState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
        chests.stateAnimator[0] = "被吸吮";
        chests.stateAnimator[1] = "被吸吮_慢";
        chests.stateAnimator[2] = "被吸吮_中";
        chests.stateAnimator[3] = "被吸吮_快";
        chests.SwitchAnimator();
        chests.haveMouth = true;
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