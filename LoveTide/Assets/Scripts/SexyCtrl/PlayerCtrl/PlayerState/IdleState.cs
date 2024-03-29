using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    public void OnEnterState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        player.StopAllActor();
        player.UICtrl.SetButtonDisplay(0);
        player.UICtrl.SetButtonLimitation(1,true);
        player.UICtrl.SetSliderLimitation(true);
        player.nowSlider = 0;
        player.motionSpeed = 0;
    }

    public void OnStayState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        player.OrgasmDetected();
    }

    public void OnExitState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        player.UICtrl.SetButtonLimitation(1,false);
        player.UICtrl.SetSliderLimitation(false);
        player.speedCtrl[0].value = 0;
    }
}
