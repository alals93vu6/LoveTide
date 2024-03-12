using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CumState_Face : IState
{
    public void OnEnterState(object action)
    {
        var hand = (SexyCtrl_Head)action;
        hand.testText.text = "G表情:高潮";
    }

    public void OnStayState(object action)
    {
        var hand = (SexyCtrl_Head)action;
        
    }

    public void OnExitState(object action)
    {
        var hand = (SexyCtrl_Head)action;
    }
}