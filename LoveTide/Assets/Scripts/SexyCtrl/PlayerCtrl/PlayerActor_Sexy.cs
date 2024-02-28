using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor_Sexy : MonoBehaviour
{

    [Header("狀態")] 
    [SerializeField] public bool isEnter;
    [SerializeField] public bool isHand;
    [SerializeField] private IState CurrenState = new IdleState();
    [SerializeField] private int actionState;

    [Header("物件")] 
    [SerializeField] public PlayerAnimatorManager animatorCtrl;
    [SerializeField] public SexyUIManager UICtrl;
    [SerializeField] public PlayerAudioManager audioCtrl;

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

    public void ChangeState(IState nextState)
    {
        Debug.Log("StateChange");
        CurrenState.OnExitState(this);
        nextState.OnEnterState(this);
        CurrenState = nextState;
    }
}


