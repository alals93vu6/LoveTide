using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMassageState_Hand : IState
{
    public void OnEnterState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        hand.stateAnimator[0] = "按摩中";
        hand.stateAnimator[1] = "按摩中_慢";
        hand.stateAnimator[2] = "按摩中_中";
        hand.stateAnimator[3] = "按摩中_快";
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