using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandJobState_Body : IState
{
    public void OnEnterState(object action)
    {
        var body = (SexyCtrl_Body)action;
        body.testText.text = "G身體:被手插入"+ "\n" + "GG:待機" ;
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
