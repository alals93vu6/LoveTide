using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_HandJob : IState
{
    public void OnEnterState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        player.animatorCtrl.rightChestsCtrl.testText.text = "P右手:已插入";
        player.animatorCtrl.bodyCtrl.testText.text = "G身體:被手插入"+ "\n" + "GG:待機" ;
        player.UICtrl.SetButtonDisplay(1);
    }

    public void OnStayState(object action)
    {
        var player = (PlayerActor_Sexy)action;
    }

    public void OnExitState(object action)
    {
        var player = (PlayerActor_Sexy)action;
    }
}
