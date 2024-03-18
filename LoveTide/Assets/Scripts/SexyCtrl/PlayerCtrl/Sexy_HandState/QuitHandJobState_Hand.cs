using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitHandJobState_Hand : IState
{
    public void OnEnterState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        hand.stateAnimator[0] = "拔出";
        hand.stateAnimator[1] = "拔出";
        hand.stateAnimator[2] = "拔出";
        hand.stateAnimator[3] = "拔出";
        hand.SwitchAnimator();
    }

    public void OnStayState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        
    }

    public void OnExitState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        
    }
}