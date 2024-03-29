using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviour
{
    [SerializeField] public SexyCtrl_Head headCtrl;
    [SerializeField] public SexyCtrl_Body bodyCtrl;
    [SerializeField] public SexyCtrl_Chests leftChestsCtrl;
    [SerializeField] public SexyCtrl_Chests rightChestsCtrl;
    [SerializeField] public SexyCtrl_Hand leftHandCtrl;
    [SerializeField] public SexyCtrl_Hand rightHandCtrl;
    [SerializeField] public Sexyctrl_GirlHand girlHandCtrl;
    [SerializeField] public int stimulationTotal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StimulationDetected();
    }

    public void StimulationDetected()
    {
        int sA ,sB ,sC ,sD;
        if (headCtrl.onKiss) { sA = 1; }else { sA = 0; } 
        if (leftChestsCtrl.stimulation) { sB = 1; }else { sB = 0; } 
        if (rightChestsCtrl.stimulation) { sC = 1; }else { sC = 0; } 
        if (bodyCtrl.nowSpeed != 0) { sD = 1; }else { sD = 0; }

        stimulationTotal = sA + sB + sC + sD;
    }

    public void OnKiss_ANCtrl()
    {
        if (headCtrl.onKiss)
        {
            headCtrl.onKiss = false;
        }
        else
        {
           headCtrl.onKiss = true;
        }
        
        if (rightChestsCtrl.haveMouth)
        {
            rightChestsCtrl.ChangeState(new IdleState_Chests());
        }
        
        if (leftChestsCtrl.haveMouth)
        {
            leftChestsCtrl.ChangeState(new IdleState_Chests());
        }
        headCtrl.SwitchAnimator();
    }

    public void OnLick_ANCtrl(bool isRight)
    {
        if (isRight)
        {
            if (rightChestsCtrl.CurrenState is OnlickState_Chests)
            {
                rightChestsCtrl.ChangeState(new IdleState_Chests());
            }
            else
            {
                rightChestsCtrl.ChangeState(new OnlickState_Chests());
                if (!FindObjectOfType<PlayerActor_Sexy>().isHand)
                {
                    rightHandCtrl.ChangeState(new IdleState_Hand());
                }

                if (leftChestsCtrl.haveMouth)
                {
                    leftChestsCtrl.ChangeState(new IdleState_Chests());
                }
                if (headCtrl.onKiss)
                {
                    headCtrl.onKiss = false;
                }
            }
        }
        else
        {
            if (leftChestsCtrl.CurrenState is OnlickState_Chests)
            {
                leftChestsCtrl.ChangeState(new IdleState_Chests());
            }
            else
            {
                leftChestsCtrl.ChangeState(new OnlickState_Chests());
                leftHandCtrl.ChangeState(new IdleState_Hand());
            
                if (rightChestsCtrl.haveMouth)
                {
                    rightChestsCtrl.ChangeState(new IdleState_Chests());
                }
                if (headCtrl.onKiss)
                {
                    headCtrl.onKiss = false;
                }
            }
        }
        headCtrl.SwitchAnimator();
    }

    public void OnGrasp_ANCtrl(bool isRight)
    {
        if (isRight)
        {
            if (rightChestsCtrl.CurrenState is OnGraspState_Chests)
            {
                rightChestsCtrl.ChangeState(new IdleState_Chests());
                rightHandCtrl.ChangeState(new IdleState_Hand());
            }
            else
            {
                rightChestsCtrl.ChangeState(new OnGraspState_Chests());
                rightHandCtrl.ChangeState(new GradHandState_Hand());
            }
        }
        else
        {
            if (leftChestsCtrl.CurrenState is OnGraspState_Chests)
            {
                leftChestsCtrl.ChangeState(new IdleState_Chests());
                leftHandCtrl.ChangeState(new IdleState_Hand());
            }
            else
            {
                leftChestsCtrl.ChangeState(new OnGraspState_Chests());
                leftHandCtrl.ChangeState(new GradHandState_Hand());
            }
        }
    }

    public void OnSuck_ANCtrl(bool isRight)
    {
        if (isRight)
        {
            if (rightChestsCtrl.CurrenState is OnSuckState_Chests)
            {
                rightChestsCtrl.ChangeState(new IdleState_Chests());
            }
            else
            {
                rightChestsCtrl.ChangeState(new OnSuckState_Chests());
                if (!FindObjectOfType<PlayerActor_Sexy>().isHand)
                {
                    rightHandCtrl.ChangeState(new IdleState_Hand());
                }
            
                if (leftChestsCtrl.haveMouth)
                {
                    leftChestsCtrl.ChangeState(new IdleState_Chests());
                }
                if (headCtrl.onKiss)
                {
                    headCtrl.onKiss = false;
                }    
            }
        }
        else
        {
            if (leftChestsCtrl.CurrenState is OnSuckState_Chests)
            {
                leftChestsCtrl.ChangeState(new IdleState_Chests());
            }
            else
            {
                leftChestsCtrl.ChangeState(new OnSuckState_Chests());
                leftHandCtrl.ChangeState(new IdleState_Hand());
            
                if (rightChestsCtrl.haveMouth)
                {
                    rightChestsCtrl.ChangeState(new IdleState_Chests());
                }
                if (headCtrl.onKiss)
                {
                    headCtrl.onKiss = false;
                }   
            }
        }
        headCtrl.SwitchAnimator();
    }

    public void OnPinch_ANCtrl(bool isRight)
    {
        if (isRight)
        {
            if (rightChestsCtrl.CurrenState is OnPinchState_Chests)
            {
                rightChestsCtrl.ChangeState(new IdleState_Chests());
                rightHandCtrl.ChangeState(new IdleState_Hand());
            }
            else
            {
                rightChestsCtrl.ChangeState(new OnPinchState_Chests());
                rightHandCtrl.ChangeState(new OnPinchState_Hand());
            }
        }
        else
        {
            if (leftChestsCtrl.CurrenState is OnPinchState_Chests)
            {
                leftChestsCtrl.ChangeState(new IdleState_Chests());
                leftHandCtrl.ChangeState(new IdleState_Hand());
            }
            else
            {
                leftChestsCtrl.ChangeState(new OnPinchState_Chests());
                leftHandCtrl.ChangeState(new OnPinchState_Hand());
            }
        }
    }

    public void OnStopHandMotion_ANCtrl(bool isRight)
    {
        if (isRight)
        {
            rightChestsCtrl.ChangeState(new IdleState_Chests());
            rightHandCtrl.ChangeState(new IdleState_Hand());
        }
        else
        {
            leftChestsCtrl.ChangeState(new IdleState_Chests());
            leftHandCtrl.ChangeState(new IdleState_Hand());
        }
    }
    
    public void OnMassage_ANCtrl() 
    {
        if (leftHandCtrl.CurrenState is OnMassageState_Hand)
        {
            leftChestsCtrl.ChangeState(new IdleState_Chests());
            leftHandCtrl.ChangeState(new IdleState_Hand());
        }
        else
        {
            leftChestsCtrl.ChangeState(new IdleState_Chests());
            leftHandCtrl.ChangeState(new OnMassageState_Hand());
        }
    }
    
    public void OnGrasp_TwoSide_ANCtrl()
    {
        rightChestsCtrl.ChangeState(new OnGraspState_Chests());
        leftChestsCtrl.ChangeState(new OnGraspState_Chests());
        leftHandCtrl.ChangeState(new OnGraspState_Hand());
        rightHandCtrl.ChangeState(new OnGraspState_Hand());
    }
            
    public void OnPinch_TwoSide_ANCtrl()
    {
        rightChestsCtrl.ChangeState(new OnPinchState_Chests());
        rightHandCtrl.ChangeState(new OnPinchState_Hand());
        leftChestsCtrl.ChangeState(new OnPinchState_Chests());
        leftHandCtrl.ChangeState(new OnPinchState_Hand());
    }
            
    public void OnInterlockingFingers_ANCtrl()
    {
        if (girlHandCtrl.CurrenState is OnHoldHandState_GirlHand)
        {
            rightChestsCtrl.ChangeState(new IdleState_Chests());
            leftChestsCtrl.ChangeState(new IdleState_Chests());
            rightHandCtrl.ChangeState(new IdleState_Hand());
            leftHandCtrl.ChangeState(new IdleState_Hand());
            girlHandCtrl.ChangeState(new IdleState_GirlHand());
        }
        else
        {
            rightChestsCtrl.ChangeState(new OnClampedState_Chests());
            leftChestsCtrl.ChangeState(new OnClampedState_Chests());
            rightHandCtrl.ChangeState(new IdleState_Hand());
            leftHandCtrl.ChangeState(new IdleState_Hand());
            girlHandCtrl.ChangeState(new OnHoldHandState_GirlHand());   
        }
    }
            
    public void OnGrabTheHands_ANCtrl()
    {
        if (girlHandCtrl.CurrenState is OnGraspState_GirlHand)
        {
            rightChestsCtrl.ChangeState(new IdleState_Chests());
            leftChestsCtrl.ChangeState(new IdleState_Chests());
            rightHandCtrl.ChangeState(new IdleState_Hand());
            leftHandCtrl.ChangeState(new IdleState_Hand());
            girlHandCtrl.ChangeState(new IdleState_GirlHand());
        }
        else
        {
            rightChestsCtrl.ChangeState(new OnClampedState_Chests());
            leftChestsCtrl.ChangeState(new OnClampedState_Chests());
            rightHandCtrl.ChangeState(new GradHandState_Hand());
            leftHandCtrl.ChangeState(new GradHandState_Hand());
            girlHandCtrl.ChangeState(new OnGraspState_GirlHand());  
        }
    }

    public void SetAnimatorMotionSpeed(float motionSpeed)
    {
        headCtrl.nowSpeed = bodyCtrl.nowSpeed = leftChestsCtrl.nowSpeed = rightChestsCtrl.nowSpeed 
            = leftChestsCtrl.nowSpeed = rightHandCtrl.nowSpeed = leftHandCtrl.nowSpeed = girlHandCtrl.nowSpeed = motionSpeed;
    }
}
