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
            player.animatorCtrl.rightChestsCtrl.testText.text = "P右手:正在插入";
            player.animatorCtrl.bodyCtrl.testText.text = "G身體:被督進去"+ "\n" + "GG:待機" ;
        }
        else
        {
            player.animatorCtrl.bodyCtrl.testText.text = "G身體:被督進去"+ "\n" + "GG:正在督進去" ;
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
                player.ChangeState(new IdleState_HandJob());
            }
            else
            {
                player.ChangeState(new IdleState_Sexy());
            }
        }
    }

    public void OnExitState(object action)
    {
        var player = (PlayerActor_Sexy)action;
    }
}
