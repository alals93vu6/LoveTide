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
    private IState CurrenState = new IdleState();

    [Header("物件")] 
    [SerializeField] public PlayerAnimatorManager animatorCtrl;
    [SerializeField] public SexyUIManager UICtrl;
    [SerializeField] public PlayerAudioManager audioCtrl;
    [SerializeField] public Slider[] speedCtrl; public int nowSlider;

    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
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
            case 1: StopActorNomal();
                Debug.Log("Nomal"); break;
            case 2: StopActorHandJob();
                Debug.Log("handJob");break;
            case 3: StopActorSexy();
                Debug.Log("sexy");break;
        }
        
    }
    private void StopActorNomal()
    {
        animatorCtrl.girlHandCtrl.ChangeState(new IdleState_GirlHand());
        animatorCtrl.headCtrl.onKiss = false;
        animatorCtrl.leftChestsCtrl.ChangeState(new IdleState_Chests());
        animatorCtrl.rightChestsCtrl.ChangeState(new IdleState_Chests());
        animatorCtrl.leftHandCtrl.ChangeState(new IdleState_Hand());
        animatorCtrl.rightHandCtrl.ChangeState(new IdleState_Hand());
    }
    
    private void StopActorHandJob()
    {
        animatorCtrl.girlHandCtrl.ChangeState(new IdleState_GirlHand());
        animatorCtrl.headCtrl.onKiss = false;
        animatorCtrl.leftChestsCtrl.ChangeState(new IdleState_Chests());
        animatorCtrl.rightChestsCtrl.ChangeState(new IdleState_Chests());
        animatorCtrl.leftHandCtrl.ChangeState(new IdleState_Hand());
    }
    
    private void StopActorSexy()
    {
        animatorCtrl.girlHandCtrl.ChangeState(new IdleState_GirlHand());
        animatorCtrl.headCtrl.onKiss = false;
        animatorCtrl.leftChestsCtrl.ChangeState(new IdleState_Chests());
        animatorCtrl.rightChestsCtrl.ChangeState(new IdleState_Chests());
        animatorCtrl.leftHandCtrl.ChangeState(new IdleState_Hand());
        animatorCtrl.rightHandCtrl.ChangeState(new IdleState_Hand());
    }

    public void SpeedDetected(int targetSlider)
    {
        if (!Input.GetMouseButton(0))
        {
            motionSpeed = speedCtrl[targetSlider].value;
        }
    }

    public void ChangeState(IState nextState)
    {
        CurrenState.OnExitState(this);
        nextState.OnEnterState(this);
        CurrenState = nextState;
    }
}


