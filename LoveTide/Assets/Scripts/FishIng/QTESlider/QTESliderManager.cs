using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QTESliderManager : MonoBehaviour
{
    

    [SerializeField] private Animator sliderAnimator;
    [SerializeField] public SliderQTE_PlayerCtrl QTECtrl;
    [SerializeField] private SliderQTE_TargetArea QTEArea;
    // Start is called before the first frame update
    void Start()
    {
        sliderAnimator = GetComponent<Animator>();
    }
    
    public void QTESliderComponent()
    {
        QTEArea.TargetAreaComponent();
        QTECtrl.PlayerMove();
    }

    public void OnQTEDetected(bool isQTE)
    {
        if (isQTE)
        {
            OnQuitQTE();
        }
        else
        {
            OnStartQTE();
        }
    }

    public async void OnStartQTE()
    {
        sliderAnimator.Play("ReadyQTE");
        await Task.Delay(1000);
        QTEArea.ChangeMoveMode();
    }

    public async void OnQuitQTE()
    {
        sliderAnimator.Play("QuitQTE");
        await Task.Delay(800);
        QTECtrl.ResetPosition();
        QTEArea.ResetPosition();
    }
}
