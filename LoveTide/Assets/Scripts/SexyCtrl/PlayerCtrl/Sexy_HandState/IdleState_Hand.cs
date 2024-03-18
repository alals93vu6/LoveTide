using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Hand : IState
{
    public void OnEnterState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        hand.stateAnimator[0] = "待機";
        hand.stateAnimator[1] = "待機";
        hand.stateAnimator[2] = "待機";
        hand.stateAnimator[3] = "待機";
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