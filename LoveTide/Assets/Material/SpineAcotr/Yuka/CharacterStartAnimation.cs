using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStartAnimation : MonoBehaviour
{
    [SpineAnimation] public string bodyIdleAnimation = "body_idle";
    [SpineAnimation] public string eyesIdleAnimation = "Eyes_Nomal";
    [SpineAnimation] public string eyebrowIdleAnimation = "EyeBrow_A"; // 新增眉毛
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
        state.SetAnimation(0, bodyIdleAnimation, true);      // 身體播放在 Track 0
        state.SetAnimation(1, eyesIdleAnimation, true);      // 眼睛播放在 Track 1
        state.SetAnimation(2, eyebrowIdleAnimation, true);   // 眉毛播放在 Track 2 (新增)
        state.SetAnimation(3, mouthIdleAnimation, true);     // 嘴巴播放在 Track 3
        state.SetAnimation(4, facIdleAnimation, true);       // 臉部播放在 Track 4
    }

    // 外部接口：動態切換表情
    public void SetEyebrowExpression(int index)
    {
        string[] eyebrowAnims = { "EyeBrow_A", "EyeBrow_B", "EyeBrow_C", "EyeBrow_D" };
        if (index >= 0 && index < eyebrowAnims.Length)
        {
            skeletonAnimation.AnimationState.SetAnimation(2, eyebrowAnims[index], true);
        }
    }

    public void SetEyesExpression(int index)
    {
        string[] eyesAnims = {"Eyes_Nomal", "Eyes_Happy", "Eyes_Alaise", "Eyes_Hrony", "Eyes_Surprise_Nomal", "Eyes_Surprise_Alaise",
  "Eyes_Surprise_Hrony"};
        if (index >= 0 && index < eyesAnims.Length)
        {
            skeletonAnimation.AnimationState.SetAnimation(1, eyesAnims[index], true);
        }
    }

    public void SetMouthExpression(int index)
    {
        string[] mouthAnims = { "Mouth_Happy", "Mouth_Blush", "Mouth_Hrony", "Mouth_Surprise", "Mouth_Cry", "Mouth_Negative" };
        if (index >= 0 && index < mouthAnims.Length)
        {
            skeletonAnimation.AnimationState.SetAnimation(3, mouthAnims[index], true);
        }
    }

    public void SetFaceExpression(int index)
    {
        string[] faceAnims = { "FaceBottoms_Nomal", "FaceBottoms_Blush", "FaceBottoms_Alaise", "FaceBottoms_Anxious" };
        if (index >= 0 && index < faceAnims.Length)
        {
            skeletonAnimation.AnimationState.SetAnimation(4, faceAnims[index], true);
        }
    }

    public void OnSetActorFace(string action)
    {
        switch (action)
        {
            case "NoActor":
                break;
            case "Happy":
                SetFullExpression(1, 1, 1, 0);
                break;
            case "Surprise":
                SetFullExpression(1, 4, 3, 0);
                break;
            case "Neutral":
                SetFullExpression(0, 0, 0, 0);
                break;
            case "Anxious":
                SetFullExpression(2, 0, 5, 2);
                break;
            case "Sad":
                SetFullExpression(2, 0, 5, 0);
                break;
            case "Angry":
                SetFullExpression(3, 0, 5, 0);
                break;
            case "Disdain":
                SetFullExpression(1, 2, 5, 2);
                break;
            case "Shy":
                SetFullExpression(3, 0, 2, 1);
                break;
            case "shock":
                SetFullExpression(0, 5, 0, 2);
                break;
        }
    }

    // 完整表情設定
    public void SetFullExpression(int eyebrowIndex, int eyesIndex, int mouthIndex, int faceIndex)
    {
        SetEyebrowExpression(eyebrowIndex);
        SetEyesExpression(eyesIndex);
        SetMouthExpression(mouthIndex);
        SetFaceExpression(faceIndex);
    }
}
