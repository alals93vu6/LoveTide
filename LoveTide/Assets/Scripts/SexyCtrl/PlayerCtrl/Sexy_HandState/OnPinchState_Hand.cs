using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPinchState_Hand : IState
{
    public void OnEnterState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        hand.stateAnimator[0] = "玩弄著胸部";
        hand.stateAnimator[1] = "玩弄著胸部_慢";
        hand.stateAnimator[2] = "玩弄著胸部_中";
        hand.stateAnimator[3] = "玩弄著胸部_快";
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