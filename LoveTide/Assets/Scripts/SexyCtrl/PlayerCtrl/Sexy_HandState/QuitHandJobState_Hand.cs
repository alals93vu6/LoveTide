﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitHandJobState_Hand : IState
{
    public void OnEnterState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        hand.testText.text = "正在拔出";
    }

    public void OnStayState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        
    }

    public void OnExitState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        
    }
}