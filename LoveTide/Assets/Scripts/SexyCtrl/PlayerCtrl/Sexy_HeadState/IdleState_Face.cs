using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Face : IState
{
    public void OnEnterState(object action)
    {
        var face = (SexyCtrl_Head)action;
        face.stateAnimator[0] = "G表情:待機";
        face.stateAnimator[1] = "G表情:害羞_慢";
        face.stateAnimator[2] = "G表情:躁動_中";
        face.stateAnimator[3] = "G表情:享受_快";
        face.stateAnimator[4] = "G表情:接吻";
        face.stateAnimator[5] = "G表情:接吻_慢";
        face.stateAnimator[6] = "G表情:接吻_中";
        face.stateAnimator[7] = "G表情:接吻_快";
        face.SwitchAnimator();
    }

    public void OnStayState(object action)
    {
        var face = (SexyCtrl_Head)action;
    }

    public void OnExitState(object action)
    {
        var face = (SexyCtrl_Head)action;
    }
}