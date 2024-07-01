using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spine.Unity;
using UnityEngine;

public class DisplayObjectCtrl : MonoBehaviour
{
    [SerializeField] public bool isStatic;
    [SerializeField] private SkeletonAnimation dynamicsAnimator;
    [SerializeField] private string[] dynamicsAnimatorNumber;
    [SerializeField] private Sprite[] staticImage;
    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }
    private void OnStart()
    {
        if (isStatic)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = staticImage[1];
        }
        else
        {
            dynamicsAnimator = GetComponent<SkeletonAnimation>();
            dynamicsAnimator.AnimationState.SetAnimation(0, dynamicsAnimatorNumber[1], true);
        }
    }

    public void OnSwitchImage(int watchPage)
    {
        if (isStatic)
        {
            ChangeStaticImage(watchPage);
        }
        else
        {
            ChangeDynamicsImage(watchPage);
        }
    }

    private void ChangeStaticImage(int setPage)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = staticImage[setPage];
    }

    private void ChangeDynamicsImage(int setPage)
    {
        dynamicsAnimator.AnimationState.SetAnimation(0, dynamicsAnimatorNumber[setPage], true);
    }

    public async void OnQuitWatchImage()
    {
        await Task.Delay(800);
        Destroy(this.gameObject);
    }
}
