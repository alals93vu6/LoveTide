using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustingState_Body : IState
{
    public void OnEnterState(object action)
    {
        var body = (SexyCtrl_Body)action;
        body.stateAnimator[0] = "G身體:被插入"+ "\n" + "GG:插入待機中";
        body.stateAnimator[1] = "G身體:被插入"+ "\n" + "GG:緩慢移動";
        body.stateAnimator[2] = "G身體:被插入"+ "\n" + "GG:插插中";
        body.stateAnimator[3] = "G身體:被插入"+ "\n" + "GG:快速抽插中";
        body.SwitchAnimator();
        body.stimulation = true;
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