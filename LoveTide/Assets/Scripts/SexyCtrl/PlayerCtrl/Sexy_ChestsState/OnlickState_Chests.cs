using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlickState_Chests : IState
{
    public void OnEnterState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
        chests.testText.text = "被舔";
        chests.haveMouth = true;
    }

    public void OnStayState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
        switch (chests.nowSpeed)
        {
            case 0: chests.testText.text = "被舔" ; break;
            case 1: chests.testText.text = "被舔_慢" ; break;
            case 2: chests.testText.text = "被舔_中" ; break;
            case 3: chests.testText.text = "被舔_快" ; break;
        }
    }

    public void OnExitState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
        chests.haveMouth = false;
    }
}