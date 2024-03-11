using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Body : IState
{
    public void OnEnterState(object action)
    {
        var body = (SexyCtrl_Body)action;
        body.testText.text = "G身體:待機"+ "\n" + "GG:待機" ;
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