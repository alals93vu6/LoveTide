using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGraspState_Hand : IState
{
    public void OnEnterState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        hand.testText.text = "抓著胸部";
    }

    public void OnStayState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
        switch (hand.nowSpeed)
        {
            case 0: hand.testText.text = "抓著胸部" ; break;
            case 1: hand.testText.text = "抓著胸部_慢" ; break;
            case 2: hand.testText.text = "抓著胸部_中" ; break;
            case 3: hand.testText.text = "抓著胸部_快" ; break;
        }
    }

    public void OnExitState(object action)
    {
        var hand = (SexyCtrl_Hand)action;
    }
}