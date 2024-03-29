using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CumState_Pair : IState
{
    private float waitTime;
    public void OnEnterState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        player.motionSpeed = 0;
    }

    public void OnStayState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        waitTime += Time.deltaTime;
        if (waitTime >= 10)
        {
            player.ChangeState(new IdleState());
        }
    }

    public void OnExitState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        player.UICtrl.SetButtonLimitation(1,true);
        player.UICtrl.SetButtonLimitation(2,true);
        player.UICtrl.SetButtonLimitation(3,true);
        player.UICtrl.SetSliderLimitation(true);
        player.numericalCtrl.OrgasmNumberSet(1);
    }
}
