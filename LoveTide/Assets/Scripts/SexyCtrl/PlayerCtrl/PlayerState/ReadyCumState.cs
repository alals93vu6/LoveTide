using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyCumState : IState
{
    private float waitTime;
    public void OnEnterState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        player.UICtrl.SetButtonLimitation(1,false);
        player.UICtrl.SetButtonLimitation(2,false);
        player.UICtrl.SetButtonLimitation(3,false);
        player.UICtrl.SetSliderLimitation(false);
        Debug.Log("ReadyDetected");
    }

    public void OnStayState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        waitTime += Time.deltaTime;
        if (waitTime >= 5)
        {
            if (player.numericalCtrl.playerDelight >= 100 && player.numericalCtrl.girlDelight >= 100)
            {
                player.ChangeState(new CumState_Pair());
            }
            else if (player.numericalCtrl.playerDelight < 100 && player.numericalCtrl.girlDelight >= 100)
            {
                player.ChangeState(new CumState_Girl());   
            }
            else if (player.numericalCtrl.playerDelight >= 100 && player.numericalCtrl.girlDelight < 100)
            {
                player.ChangeState(new CumState_player());
            }
        }
    }

    public void OnExitState(object action)
    {
        var player = (PlayerActor_Sexy)action;
        Debug.Log("CumDetected");
    }
}
