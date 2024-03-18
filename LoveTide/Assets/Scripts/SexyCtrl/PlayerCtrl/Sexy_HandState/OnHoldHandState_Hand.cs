using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHoldHandState_Hand : IState
{
    public void OnEnterState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        hand.stateAnimator[0] = "十指緊扣";
        hand.stateAnimator[1] = "十指緊扣_慢";
        hand.stateAnimator[2] = "十指緊扣_中";
        hand.stateAnimator[3] = "十指緊扣_快";
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