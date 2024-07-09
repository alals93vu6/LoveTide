using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QTESliderManager : MonoBehaviour
{
    [SerializeField] public bool QTEon;
    [SerializeField] public bool isStop;

    [SerializeField] private Animator sliderAnimator;
    [SerializeField] private SliderQTE_PlayerCtrl QTECtrl;
    [SerializeField] private SliderQTE_TargetArea QTEArea;
    // Start is called before the first frame update
    void Start()
    {
        sliderAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnQTEDetected();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isStop)
            {
                isStop = false;
            }
            else
            {
                isStop = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isStop && QTEon)
        {
            QTEArea.TargetAreaComponent();
            QTECtrl.PlayerMove();
        }
    }

    private void OnQTEDetected()
    {
        if (QTEon)
        {
            OnQuitQTE();
        }
        else
        {
            OnStartQTE();
        }
    }

    private async void OnStartQTE()
    {
        sliderAnimator.Play("ReadyQTE");
        QTEon = true;
        await Task.Delay(1000);
        QTEArea.ChangeMoveMode();
    }

    private async void OnQuitQTE()
    {
        sliderAnimator.Play("QuitQTE");
        QTEon = false;
        await Task.Delay(800);
        QTECtrl.ResetPosition();
        QTEArea.ResetPosition();
    }
}
