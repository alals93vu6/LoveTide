using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStartAnimation : MonoBehaviour
{
    [SpineAnimation] public string bodyIdleAnimation = "body_idle";
    [SpineAnimation] public string eyesIdleAnimation = "Eyes_Nomal";
    [SpineAnimation] public string mouthIdleAnimation = "Mouth_Happy";
    [SpineAnimation] public string facIdleAnimation = "FaceBottoms_Blush";

    private SkeletonAnimation skeletonAnimation;

    private void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (skeletonAnimation == null)
        {
            Debug.LogError("找不到 SkeletonAnimation 元件！");
            return;
        }

        var state = skeletonAnimation.AnimationState;

        // 在不同的 Track 撥放不同部位動畫
        state.SetAnimation(0, bodyIdleAnimation, true);   // 身體播放在 Track 0
        state.SetAnimation(1, eyesIdleAnimation, true);   // 眼睛播放在 Track 1
        state.SetAnimation(2, mouthIdleAnimation, true);  // 嘴巴播放在 Track 2
        state.SetAnimation(3, facIdleAnimation, true);
    }
}
