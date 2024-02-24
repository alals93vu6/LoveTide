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
    [SerializeField] private SexyCtrl_Head headCtrl;
    [SerializeField] private SexyCtrl_Body bodyCtrl;
    [SerializeField] private SexyCtrl_LHand leftHandCtrl;
    [SerializeField] private SexyCtrl_RHand rightHandCtrl;
    [SerializeField] private Sexyctrl_GirlHand girlHandCtrl;
    // Start is called before the first frame update
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
    
    public class  ButtonActor : MonoBehaviour
    {
        public void StopAllMotion()
        {
            
        }

        public void OnKiss()
        {
            
        }

        public void OnLick(bool isRight)
        {
            
        }
        
        public void OnGrasp(bool isRight)
        {
            
        }
        
        public void OnSuck(bool isRight)
        {
            
        }
        
        public void OnPinch(bool isRight)
        {
            
        }

        public void StopHandMotion(bool isRight)
        {
            
        }

        public void OnMassage()
        {
            
        }

        public void OnGrasp_TwoSide()
        {
            
        }
        
        public void OnPinch_TwoSide()
        {
            
        }
        
        public void OnInterlockingFingers()
        {
            
        }
        
        public void OnGrabTheHands()
        {
            
        }

        public void OnStartEnter(bool isHand)
        {
            
        }
        
        public void OnStartQuit(bool isHand)
        {
            
        }
    }
}
