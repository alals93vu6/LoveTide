using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing_StruggleState : IState
{
    public void OnEnterState(object action)
    {
        var manager = (Fishingmanager)action;
        
    }

    public void OnStayState(object action)
    {
        var manager = (Fishingmanager)action;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            manager.ChangeState(new Fishing_QTEState());
        }

        if (manager.nowStamina <= 0)
        {
            manager.ChangeState(new Fishing_SettleState());
        }
    }

    public void OnExitState(object action)
    {
        var manager = (Fishingmanager)action;
        
    }
}
