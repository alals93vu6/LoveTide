using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Chests : IState
{
    public void OnEnterState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
        chests.stateAnimator[0] = "待機";
        chests.stateAnimator[1] = "待機_慢";
        chests.stateAnimator[2] = "待機_中";
        chests.stateAnimator[3] = "待機_慢";
        chests.SwitchAnimator();
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