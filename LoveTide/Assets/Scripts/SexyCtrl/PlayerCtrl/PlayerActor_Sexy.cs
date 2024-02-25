using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor_Sexy : MonoBehaviour
{

    [Header("狀態")] 
    [SerializeField] private bool A;
    [SerializeField] private IState CurrenState = new IdleState();
    [SerializeField] private int actionState;

    [Header("物件")] 
    [SerializeField] private PlayerAnimatorManager animatorCtrl;
    [SerializeField] private PlayerAudioManager audioCtrl;

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
    
    public void ChangeState(IState nextState)
    {
        //Debug.Log("StateChange");
        CurrenState.OnExitState(this);
        nextState.OnEnterState(this);
        CurrenState = nextState;
    }
}


