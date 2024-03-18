using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustingState_Face : IState
{
    public void OnEnterState(object action)
    {
        var face = (SexyCtrl_Head)action;
        face.stateAnimator[0] = "G表情:害羞";
        face.stateAnimator[1] = "G表情:獻媚_慢";
        face.stateAnimator[2] = "G表情:享受_中";
        face.stateAnimator[3] = "G表情:沉淪_快";
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