using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClampedState_Chests : IState
{
    public void OnEnterState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
        chests.testText.text = "被夾住";
    }

    public void OnStayState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
        switch (chests.nowSpeed)
        {
            case 0: chests.testText.text = "被夾住" ; break;
            case 1: chests.testText.text = "被夾住_慢" ; break;
            case 2: chests.testText.text = "被夾住_中" ; break;
            case 3: chests.testText.text = "被夾住_快" ; break;
        }
    }

    public void OnExitState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
    }
}