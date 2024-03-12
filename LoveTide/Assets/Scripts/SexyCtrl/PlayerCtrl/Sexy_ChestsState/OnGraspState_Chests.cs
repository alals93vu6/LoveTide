using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGraspState_Chests : IState
{
    public void OnEnterState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
        chests.testText.text = "被抓著";
    }

    public void OnStayState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
        switch (chests.nowSpeed)
        {
            case 0: chests.testText.text = "被抓著" ; break;
            case 1: chests.testText.text = "被抓著_慢" ; break;
            case 2: chests.testText.text = "被抓著_中" ; break;
            case 3: chests.testText.text = "被抓著_快" ; break;
        }
    }

    public void OnExitState(object action)
    {
        var chests = (SexyCtrl_Chests)action;
    }
}