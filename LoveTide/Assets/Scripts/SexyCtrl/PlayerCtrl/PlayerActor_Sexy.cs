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
    [SerializeField] private IState CurrenState = new IdleState();
    [SerializeField] private int actionState;

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
        animatorCtrl.girlHandCtrl.testText.text = "G手部:待機";
        animatorCtrl.headCtrl.testText.text = "G表情:待機";
        animatorCtrl.leftChestsCtrl.testText.text = "G左胸:待機";
        animatorCtrl.rightChestsCtrl.testText.text = "G右胸:待機";
        animatorCtrl.leftHandCtrl.testText.text = "P左手:待機";
        animatorCtrl.rightHandCtrl.testText.text = "P右手:待機";
        animatorCtrl.bodyCtrl.testText.text = "G身體:待機"+ "\n" + "GG:待機";
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


