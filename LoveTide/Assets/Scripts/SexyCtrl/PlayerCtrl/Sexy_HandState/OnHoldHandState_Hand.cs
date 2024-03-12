using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHoldHandState_Hand : IState
{
    public void OnEnterState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        hand.testText.text = "十指緊扣";
    }

    public void OnStayState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        switch (hand.nowSpeed)
        {
            case 0: hand.testText.text = "十指緊扣" ; break;
            case 1: hand.testText.text = "十指緊扣_慢" ; break;
            case 2: hand.testText.text = "十指緊扣_中" ; break;
            case 3: hand.testText.text = "十指緊扣_快" ; break;
        }
    }

    public void OnExitState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
    }
}