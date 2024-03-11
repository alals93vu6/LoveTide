using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnterState_Body : IState
{
    public void OnEnterState(object action)
    {
        var body = (SexyCtrl_Body)action;
        if (GameObject.FindObjectOfType<PlayerActor_Sexy>().isHand)
        {
            body.testText.text = "G身體:被督進去"+ "\n" + "GG:待機" ;
        }
        else
        {
            body.testText.text = "G身體:被督進去"+ "\n" + "GG:正在督進去" ;
        }
    }

    public void OnStayState(object action)
    {
        var body = (SexyCtrl_Body)action;
        var player = GameObject.FindObjectOfType<PlayerActor_Sexy>();
        if (player.isHand)
        {
            body.ChangeState(new HandJobState_Body());
        }
        else
        {
            body.ChangeState(new ThrustingState_Body());
        }
    }

    public void OnExitState(object action)
    {
        var body = (SexyCtrl_Body)action;

    }
}