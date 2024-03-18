using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnQuitState_Body : IState
{
    public void OnEnterState(object action)
    {
        var body = (SexyCtrl_Body)action;
        if (GameObject.FindObjectOfType<PlayerActor_Sexy>().isHand)
        {
            body.stateAnimator[0] = "G身體:手被拔出"+ "\n" + "GG:待機" ;
        }
        else
        {
            body.stateAnimator[0] = "G身體:被拔出" + "\n" + "GG:拔出中" ;
        }
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