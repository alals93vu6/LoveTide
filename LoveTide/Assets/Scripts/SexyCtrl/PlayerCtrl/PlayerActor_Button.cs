using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor_Button : MonoBehaviour
{
    [SerializeField] private PlayerActor_Sexy player;
    
    public void StopAllMotion()
    {
        player.StopAllActor();
    }
    
    public void OnKiss()
    {
        if (player.animatorCtrl.headCtrl.onKiss)
        {
            player.animatorCtrl.headCtrl.onKiss = false;
        }
        else
        {
            player.animatorCtrl.headCtrl.onKiss = true;
        }
        if (player.animatorCtrl.rightChestsCtrl.haveMouth)
        {
            player.animatorCtrl.rightChestsCtrl.ChangeState(new IdleState_Chests());
        }
        if (player.animatorCtrl.leftChestsCtrl.haveMouth)
        {
            player.animatorCtrl.leftChestsCtrl.ChangeState(new IdleState_Chests());
        }
    }
    
    public void OnLick(bool isRight)
    {
        if (isRight)
        {
            player.animatorCtrl.rightChestsCtrl.ChangeState(new OnlickState_Chests());
            if (!player.isHand)
            {
                player.animatorCtrl.rightHandCtrl.ChangeState(new IdleState_Hand());
            }

            if (player.animatorCtrl.leftChestsCtrl.haveMouth)
            {
                
                player.animatorCtrl.leftChestsCtrl.ChangeState(new IdleState_Chests());
            }
            if (player.animatorCtrl.headCtrl.onKiss)
            {
                player.animatorCtrl.headCtrl.onKiss = false;
            }
        }
        else
        {
            player.animatorCtrl.leftChestsCtrl.ChangeState(new OnlickState_Chests());
            player.animatorCtrl.leftHandCtrl.ChangeState(new IdleState_Hand());
            
            if (player.animatorCtrl.rightChestsCtrl.haveMouth)
            {
                player.animatorCtrl.rightChestsCtrl.ChangeState(new IdleState_Chests());
            }
            if (player.animatorCtrl.headCtrl.onKiss)
            {
                player.animatorCtrl.headCtrl.onKiss = false;
            }
        }
    }
            
    public void OnGrasp(bool isRight)
    {
        if (isRight)
        {
            player.animatorCtrl.rightChestsCtrl.ChangeState(new OnGraspState_Chests());
            player.animatorCtrl.rightHandCtrl.ChangeState(new GradHandState_Hand());
        }
        else
        {
            player.animatorCtrl.leftChestsCtrl.ChangeState(new OnGraspState_Chests());
            player.animatorCtrl.leftHandCtrl.ChangeState(new OnGraspState_Hand());
        }
    }
            
    public void OnSuck(bool isRight)
    {
        if (isRight)
        {
            player.animatorCtrl.rightChestsCtrl.ChangeState(new OnSuckState_Chests());
            if (!player.isHand)
            {
                player.animatorCtrl.rightHandCtrl.ChangeState(new IdleState_Hand());
            }
            
            if (player.animatorCtrl.leftChestsCtrl.haveMouth)
            {
                player.animatorCtrl.leftChestsCtrl.ChangeState(new IdleState_Chests());
            }
            if (player.animatorCtrl.headCtrl.onKiss)
            {
                player.animatorCtrl.headCtrl.onKiss = false;
            }
        }
        else
        {
            player.animatorCtrl.leftChestsCtrl.ChangeState(new OnSuckState_Chests());
            player.animatorCtrl.leftHandCtrl.ChangeState(new IdleState_Hand());
            
            if (player.animatorCtrl.rightChestsCtrl.haveMouth)
            {
                player.animatorCtrl.rightChestsCtrl.ChangeState(new IdleState_Chests());
            }
            if (player.animatorCtrl.headCtrl.onKiss)
            {
                player.animatorCtrl.headCtrl.onKiss = false;
            }
        }
    }
            
    public void OnPinch(bool isRight)
    {
        if (isRight)
        {
            player.animatorCtrl.rightChestsCtrl.ChangeState(new OnPinchState_Chests());
            player.animatorCtrl.rightHandCtrl.ChangeState(new OnPinchState_Hand());
        }
        else
        {
            player.animatorCtrl.leftChestsCtrl.ChangeState(new OnPinchState_Chests());
            player.animatorCtrl.leftHandCtrl.ChangeState(new OnPinchState_Hand());
        }
    }
    
    public void StopHandMotion(bool isRight)
    {
        if (isRight)
        {
            player.animatorCtrl.rightChestsCtrl.ChangeState(new IdleState_Chests());
            player.animatorCtrl.rightHandCtrl.ChangeState(new IdleState_Hand());
        }
        else
        {
            player.animatorCtrl.leftChestsCtrl.ChangeState(new IdleState_Chests());
            player.animatorCtrl.leftHandCtrl.ChangeState(new IdleState_Hand());
        }
    }
    
    public void OnMassage() 
    {
        player.animatorCtrl.leftChestsCtrl.ChangeState(new IdleState_Chests());
        player.animatorCtrl.leftHandCtrl.ChangeState(new OnMassageState_Hand());
    }
    
    public void OnGrasp_TwoSide()
    {
        player.animatorCtrl.rightChestsCtrl.ChangeState(new OnGraspState_Chests());
        player.animatorCtrl.leftChestsCtrl.ChangeState(new OnGraspState_Chests());
        player.animatorCtrl.leftHandCtrl.ChangeState(new OnGraspState_Hand());
        player.animatorCtrl.rightHandCtrl.ChangeState(new OnGraspState_Hand());
    }
            
    public void OnPinch_TwoSide()
    {
        player.animatorCtrl.rightChestsCtrl.ChangeState(new OnPinchState_Chests());
        player.animatorCtrl.rightHandCtrl.ChangeState(new OnPinchState_Hand());
        player.animatorCtrl.leftChestsCtrl.ChangeState(new OnPinchState_Chests());
        player.animatorCtrl.leftHandCtrl.ChangeState(new OnPinchState_Hand());
    }
            
    public void OnInterlockingFingers()
    {
        player.animatorCtrl.rightChestsCtrl.ChangeState(new OnClampedState_Chests());
        player.animatorCtrl.leftChestsCtrl.ChangeState(new OnClampedState_Chests());
        player.animatorCtrl.rightHandCtrl.ChangeState(new IdleState_Hand());
        player.animatorCtrl.leftHandCtrl.ChangeState(new IdleState_Hand());
        player.animatorCtrl.girlHandCtrl.ChangeState(new OnHoldHandState_GirlHand());
    }
            
    public void OnGrabTheHands()
    {
        player.animatorCtrl.rightChestsCtrl.ChangeState(new OnClampedState_Chests());
        player.animatorCtrl.leftChestsCtrl.ChangeState(new OnClampedState_Chests());
        player.animatorCtrl.rightHandCtrl.ChangeState(new GradHandState_Hand());
        player.animatorCtrl.leftHandCtrl.ChangeState(new GradHandState_Hand());
        player.animatorCtrl.girlHandCtrl.ChangeState(new OnGraspState_GirlHand());
    }
    
    public void OnStartEnter(bool isHand)
    {
        //Debug.Log("OnStartEnter");
        player.StopAllActor();
        if (isHand) {player.isHand = true;}
        player.ChangeState(new OnStartEnterState());
    }
            
    public void OnStartQuit()
    {
        //Debug.Log("OnStartQuit");
        player.StopAllActor();
        player.ChangeState(new OnQuitState());
    }
}
