using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnQuitState : IState
{
    private float quitTime;
    public void OnEnterState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        if (player.isHand)
        {
            player.animatorCtrl.rightHandCtrl.ChangeState(new QuitHandJobState_Hand());
            player.animatorCtrl.bodyCtrl.ChangeState(new OnQuitState_Body());
            player.speedCtrl[1].value = 0;
        }
        else
        {
            player.animatorCtrl.bodyCtrl.ChangeState(new OnQuitState_Body());
            player.speedCtrl[2].value = 0;
        }
        quitTime = 0;
    }

    public void OnStayState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        quitTime += Time.deltaTime;
        if (quitTime >= 1.5f)
        {
            player.ChangeState(new IdleState());
        }
    }

    public void OnExitState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        player.isHand = false;
        player.isEnter = false;
    }
}
