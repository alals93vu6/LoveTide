using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_GirlHand : IState
{
    public void OnEnterState(object action)
    {
        var girlHand = (Sexyctrl_GirlHand)action;
        girlHand.testText.text = "G手部:待機";
    }

    public void OnStayState(object action)
    {
        var girlHand = (Sexyctrl_GirlHand)action;
        switch (girlHand.nowSpeed)
        {
            case 0: girlHand.testText.text = "G手部:待機" ; break;
            case 1: girlHand.testText.text = "G手部:待機_慢" ; break;
            case 2: girlHand.testText.text = "G手部:待機_中" ; break;
            case 3: girlHand.testText.text = "G手部:待機_快" ; break;
        }
    }

    public void OnExitState(object action)
    {
        var girlHand = (Sexyctrl_GirlHand)action;
    }
}