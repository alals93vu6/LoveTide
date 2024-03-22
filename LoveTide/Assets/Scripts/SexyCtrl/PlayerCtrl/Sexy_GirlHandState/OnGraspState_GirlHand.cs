using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGraspState_GirlHand : IState
{
    public void OnEnterState(object action)
    {
        var girlHand = (Sexyctrl_GirlHand)action;
        girlHand.stateAnimator[0] = "G手部:被抓住";
        girlHand.stateAnimator[1] = "G手部:被抓住_慢";
        girlHand.stateAnimator[2] = "G手部:被抓住_中";
        girlHand.stateAnimator[3] = "G手部:被抓住_快";
        girlHand.SwitchAnimator();
        girlHand.OnLimitation(false);
    }

    public void OnStayState(object action)
    {
        var girlHand = (Sexyctrl_GirlHand)action;
    }

    public void OnExitState(object action)
    {
        var girlHand = (Sexyctrl_GirlHand)action;
        girlHand.OnLimitation(true);
    }
}