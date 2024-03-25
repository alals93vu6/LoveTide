using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Body : IState
{
    public void OnEnterState(object action)
    {
        var body = (SexyCtrl_Body)action;
        body.stateAnimator[0] = "G身體:待機"+ "\n" + "GG:待機";
        body.stateAnimator[1] = "G身體:待機"+ "\n" + "GG:緩慢挑動";
        body.stateAnimator[2] = "G身體:待機"+ "\n" + "GG:摩擦中";
        body.stateAnimator[3] = "G身體:待機"+ "\n" + "GG:高速摩擦";
        body.SwitchAnimator();
        body.stimulation = false;
    }

    public void OnStayState(object action)
    {
        var body = (SexyCtrl_Body)action;
        
    }

    public void OnExitState(object action)
    {
        var body = (SexyCtrl_Body)action;
        
    }
}