using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CumState_Body : IState
{
    public void OnEnterState(object action)
    {
        var body = (SexyCtrl_Body)action;
        
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