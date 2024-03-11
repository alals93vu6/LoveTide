using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_GirlHand : IState
{
    public void OnEnterState(object action)
    {
        var gHand = (Sexyctrl_GirlHand)action;
        gHand.testText.text = "G手部:待機";
    }

    public void OnStayState(object action)
    {
        
    }

    public void OnExitState(object action)
    {
        
    }
}