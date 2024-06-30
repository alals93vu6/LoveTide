using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YukaIdleState : IState
{
    private float passTime;
    private bool readySwitchPosition;
    
    public void OnEnterState(object action)
    {
        var yuka = (YukaManager)action;
        var rangeNumber = Random.Range(0, 2);
        yuka.yukaAnimator.AnimationState.SetAnimation(0, yuka.idleAnimator[rangeNumber], true);
        Debug.Log(yuka.walkAnimator[rangeNumber]);
        passTime = 0f;
        readySwitchPosition = false;
    }

    public void OnStayState(object action)
    {
        var yuka = (YukaManager)action;
        passTime += Time.deltaTime;
        
        if (passTime >= 6)
        {
            readySwitchPosition = true;
        }

        if (passTime >= 6 && readySwitchPosition)
        {
            yuka.SwitchPosition();
            yuka.ChangeState(new YukaMoveState());
        }
    }

    public void OnExitState(object action)
    {
        var yuka = (YukaManager)action;
    }
}
