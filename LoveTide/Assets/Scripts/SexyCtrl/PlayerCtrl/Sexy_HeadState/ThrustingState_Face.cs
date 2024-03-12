using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustingState_Face : IState
{
    public void OnEnterState(object action)
    {
        var hand = (SexyCtrl_Head)action;
        hand.testText.text = "G表情:害羞";
    }

    public void OnStayState(object action)
    {
        var hand = (SexyCtrl_Head)action;
        if (hand.onKiss)
        {
            switch (hand.nowSpeed)
            {
                case 0: hand.testText.text = "G表情:接吻" ; break;
                case 1: hand.testText.text = "G表情:接吻_慢" ; break;
                case 2: hand.testText.text = "G表情:接吻_中" ; break;
                case 3: hand.testText.text = "G表情:接吻_快" ; break;
            }  
        }
        else
        {
            switch (hand.nowSpeed)
            {
                case 0: hand.testText.text = "G表情:害羞" ; break;
                case 1: hand.testText.text = "G表情:獻媚_慢" ; break;
                case 2: hand.testText.text = "G表情:享受_中" ; break;
                case 3: hand.testText.text = "G表情:沉淪_快" ; break;
            }  
        }
    }

    public void OnExitState(object action)
    {
        var hand = (SexyCtrl_Head)action;
    }
}