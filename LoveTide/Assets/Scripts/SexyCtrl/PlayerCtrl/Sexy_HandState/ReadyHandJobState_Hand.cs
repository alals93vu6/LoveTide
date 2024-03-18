using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyHandJobState_Hand : IState
{
    public void OnEnterState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        hand.stateAnimator[0] = "插入";
        hand.stateAnimator[1] = "插入";
        hand.stateAnimator[2] = "插入";
        hand.stateAnimator[3] = "插入";
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