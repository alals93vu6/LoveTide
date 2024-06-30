using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YukaMoveState : IState
{
    private bool isCheck;
    public void OnEnterState(object action)
    {
        var yuka = (YukaManager)action;
        var rangeNumber = Random.Range(0, 2);
        yuka.yukaAnimator.AnimationState.SetAnimation(0, yuka.walkAnimator[rangeNumber], true);
        Debug.Log(yuka.walkAnimator[rangeNumber]);
        isCheck = false;
    }

    public void OnStayState(object action)
    {
        var yuka = (YukaManager)action;
        if (yuka.wayDistance >= 5f && !isCheck)
        {
            yuka.gameObject.transform.position = Vector3.MoveTowards(yuka.gameObject.transform.position,yuka.wayPoint[yuka.wayTarget].transform.position,55 * Time.deltaTime);
        }
        else
        {
            yuka.ChangeState(new YukaIdleState());
            isCheck = true;
        }
    }

    public void OnExitState(object action)
    {
        var yuka = (YukaManager)action;
    }
}