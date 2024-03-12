using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandJobState_Hand : IState
{
    public void OnEnterState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        hand.testText.text = "已插入";
    }

    public void OnStayState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        switch (hand.nowSpeed)
        {
            case 0: hand.testText.text = "已插入" ; break;
            case 1: hand.testText.text = "緩慢挑逗著" ; break;
            case 2: hand.testText.text = "普通速度挑逗著" ; break;
            case 3: hand.testText.text = "快速抽插著" ; break;
        }
    }

    public void OnExitState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
    }
}