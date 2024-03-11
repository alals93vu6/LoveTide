using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustingState : IState
{
    public void OnEnterState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        player.animatorCtrl.bodyCtrl.testText.text = "G身體:被插入"+ "\n" + "GG:插入待機中" ;
        player.UICtrl.SetButtonDisplay(2);
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
