using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStartEnterState : IState
{
    private float passTime;
    public void OnEnterState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        player.isEnter = true;
        player.UICtrl.SetButtonDisplay(3);
        if (player.isHand)
        {
            player.animatorCtrl.rightHandCtrl.ChangeState(new ReadyHandJobState_Hand());
            player.animatorCtrl.bodyCtrl.ChangeState(new OnEnterState_Body());
        }
        else
        {
            player.animatorCtrl.bodyCtrl.ChangeState(new OnEnterState_Body());
        }
        passTime = 0;
    }

    public void OnStayState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        passTime += Time.deltaTime;
        if (passTime >= 1.5f)
        {
            if (player.isHand)
            {
                player.ChangeState(new HandJobState());
            }
            else
            {
                player.ChangeState(new ThrustingState());
            }
        }
    }

    public void OnExitState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        if (player.isHand)
        {
            player.nowSlider = 1;
        }
        else
        {
            player.nowSlider = 2;
        }
    }
}
