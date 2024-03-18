using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnterState_Face : IState
{
    public void OnEnterState(object action)
    {
        var face = (SexyCtrl_Head)action;
        face.stateAnimator[0] = "G表情:忍耐";
        face.stateAnimator[1] = "G表情:忍耐";
        face.stateAnimator[2] = "G表情:忍耐";
        face.stateAnimator[3] = "G表情:忍耐";
        face.stateAnimator[4] = "G表情:忍耐";
        face.stateAnimator[5] = "G表情:忍耐";
        face.stateAnimator[6] = "G表情:忍耐";
        face.stateAnimator[7] = "G表情:忍耐";
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