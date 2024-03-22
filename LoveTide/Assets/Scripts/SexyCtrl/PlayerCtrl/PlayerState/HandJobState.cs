using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandJobState : IState
{
    public void OnEnterState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        player.animatorCtrl.rightHandCtrl.ChangeState(new HandJobState_Hand());
        player.animatorCtrl.bodyCtrl.ChangeState(new HandJobState_Body());
        player.UICtrl.SetButtonDisplay(1);
        player.UICtrl.SetButtonLimitation(2,true);
        player.UICtrl.SetSliderLimitation(true);
    }

    public void OnStayState(object action)
    {
        var player = (PlayerActor_Sexy)action;
    }

    public void OnExitState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        player.UICtrl.SetButtonLimitation(2,false);
        player.UICtrl.SetSliderLimitation(false);
    }
}
