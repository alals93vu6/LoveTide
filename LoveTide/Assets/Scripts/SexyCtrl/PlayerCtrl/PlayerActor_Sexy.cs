using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActor_Sexy : MonoBehaviour
{

    [Header("狀態")] 
    [SerializeField] public bool isEnter;
    [SerializeField] public bool isHand;
    [SerializeField] public float motionSpeed;
    [SerializeField] private int actionState;
    public IState CurrenState = new IdleState();

    [Header("物件")] 
    [SerializeField] public PlayerAnimatorManager animatorCtrl;
    [SerializeField] public SexyUIManager UICtrl;
    [SerializeField] public PlayerAudioManager audioCtrl;
    [SerializeField] public Slider[] speedCtrl; public int nowSlider;
    
    void Update()
    {
        CurrenState.OnStayState(this);
        SpeedDetected(nowSlider);
    }

    public void StopAllActor()
    {
        var actorDetected = 0;
        if (!isEnter && !isHand)
        {
            actorDetected = 1;
        }
        else if (isEnter && isHand)
        {
            actorDetected = 2; 
        }
        else if (isEnter && !isHand)
        {
            actorDetected = 3;
        }

        speedCtrl[nowSlider].value = 0;
        switch (actorDetected)
        {
            case 1: StopActorNomal(); break;
            case 2: StopActorHandJob(); ;break;
            case 3: StopActorSexy(); ;break;
        }
        animatorCtrl.headCtrl.SwitchAnimator();
    }
    private void StopActorNomal()
    {
        animatorCtrl.girlHandCtrl.ChangeState(new IdleState_GirlHand());
        animatorCtrl.headCtrl.onKiss = false;
        animatorCtrl.leftChestsCtrl.ChangeState(new IdleState_Chests());
        animatorCtrl.rightChestsCtrl.ChangeState(new IdleState_Chests());
        animatorCtrl.leftHandCtrl.ChangeState(new IdleState_Hand());
        animatorCtrl.rightHandCtrl.ChangeState(new IdleState_Hand());
        animatorCtrl.bodyCtrl.ChangeState(new IdleState_Body());
    }
    
    private void StopActorHandJob()
    {
        animatorCtrl.girlHandCtrl.ChangeState(new IdleState_GirlHand());
        animatorCtrl.headCtrl.onKiss = false;
        animatorCtrl.leftChestsCtrl.ChangeState(new IdleState_Chests());
        animatorCtrl.rightChestsCtrl.ChangeState(new IdleState_Chests());
        animatorCtrl.leftHandCtrl.ChangeState(new IdleState_Hand());
        animatorCtrl.bodyCtrl.ChangeState(new IdleState_Body());
    }
    
    private void StopActorSexy()
    {
        animatorCtrl.girlHandCtrl.ChangeState(new IdleState_GirlHand());
        animatorCtrl.headCtrl.onKiss = false;
        animatorCtrl.leftChestsCtrl.ChangeState(new IdleState_Chests());
        animatorCtrl.rightChestsCtrl.ChangeState(new IdleState_Chests());
        animatorCtrl.leftHandCtrl.ChangeState(new IdleState_Hand());
        animatorCtrl.rightHandCtrl.ChangeState(new IdleState_Hand());
        animatorCtrl.bodyCtrl.ChangeState(new IdleState_Body());
    }

    private void SpeedDetected(int targetSlider)
    {
        if (Input.GetMouseButtonUp(0))
        {
            motionSpeed = speedCtrl[targetSlider].value;
            animatorCtrl.SetAnimatorMotionSpeed(motionSpeed);
            //Debug.Log("SpeedDetected");
        }
    }

    public void ChangeState(IState nextState)
    {
        CurrenState.OnExitState(this);
        nextState.OnEnterState(this);
        CurrenState = nextState;
    }
}


