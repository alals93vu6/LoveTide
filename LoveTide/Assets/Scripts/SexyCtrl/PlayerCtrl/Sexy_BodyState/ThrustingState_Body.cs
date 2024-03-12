using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustingState_Body : IState
{
    public void OnEnterState(object action)
    {
        var body = (SexyCtrl_Body)action;
        body.testText.text = "G身體:被插入"+ "\n" + "GG:插入待機中" ;
    }

    public void OnStayState(object action)
    {
        var body = (SexyCtrl_Body)action;
        switch (body.nowSpeed)
        {
            case 0: body.testText.text = "G身體:被插入"+ "\n" + "GG:插入待機中" ; break;
            case 1: body.testText.text = "G身體:被插入"+ "\n" + "GG:緩慢移動" ; break;
            case 2: body.testText.text = "G身體:被插入"+ "\n" + "GG:插插中" ; break;
            case 3: body.testText.text = "G身體:被插入"+ "\n" + "GG:快速抽插中" ; break;
        }
    }

    public void OnExitState(object action)
    {
        var body = (SexyCtrl_Body)action;

    }
}