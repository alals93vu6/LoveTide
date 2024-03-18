using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CumState_Body : IState
{
    public void OnEnterState(object action)
    {
        var body = (SexyCtrl_Body)action;
        body.stateAnimator[0] = "";
        body.stateAnimator[1] = "";
        body.stateAnimator[2] = "";
        body.stateAnimator[3] = "";
        body.stateAnimator[4] = "";
        body.stateAnimator[5] = "";
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