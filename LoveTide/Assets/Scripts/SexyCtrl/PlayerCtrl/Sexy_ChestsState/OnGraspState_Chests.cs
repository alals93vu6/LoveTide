using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGraspState_Chests : IState
{
    public void OnEnterState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
        chests.stateAnimator[0] = "被抓著";
        chests.stateAnimator[1] = "被抓著_慢";
        chests.stateAnimator[2] = "被抓著_中";
        chests.stateAnimator[3] = "被抓著_快";
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