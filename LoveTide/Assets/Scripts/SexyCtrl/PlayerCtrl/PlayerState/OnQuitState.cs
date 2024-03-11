using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnQuitState : IState
{
    private float quitTime;
    public void OnEnterState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        player.UICtrl.SetButtonDisplay(3);
        if (player.isHand)
        {
            player.animatorCtrl.rightChestsCtrl.testText.text = "P右手:正在拔出";
            player.animatorCtrl.bodyCtrl.testText.text = "G身體:手被拔出"+ "\n" + "GG:待機" ;
            player.speedCtrl[1].value = 0;
        }
        else
        {
            player.animatorCtrl.bodyCtrl.testText.text = "G身體:被拔出" + "\n" + "GG:拔出中";
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
