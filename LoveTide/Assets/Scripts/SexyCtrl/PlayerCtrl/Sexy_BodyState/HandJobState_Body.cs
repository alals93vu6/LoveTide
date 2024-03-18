using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandJobState_Body : IState
{
    public void OnEnterState(object action)
    {
        var body = (SexyCtrl_Body)action;
        body.stateAnimator[0] = "G身體:被手插入"+ "\n" + "GG:待機";
        body.stateAnimator[1] = "G身體:被手插入_慢"+ "\n" + "GG:待機";
        body.stateAnimator[2] = "G身體:被手插入_中"+ "\n" + "GG:待機";
        body.stateAnimator[3] = "G身體:被手插入_快"+ "\n" + "GG:待機";
        body.SwitchAnimator();
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
