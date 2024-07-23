using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing_QTEState : IState
{
    public void OnEnterState(object action)
    {
        var manager = (Fishingmanager)action;
        manager.isStop = true;
    }

    public void OnStayState(object action)
    {
        var manager = (Fishingmanager)action;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            manager.isStop = true;
            manager.ChangeState(new Fishing_StruggleState());
        }
    }

    public void OnExitState(object action)
    {
        var manager = (Fishingmanager)action;
        manager.isStop = false;
    }
}
