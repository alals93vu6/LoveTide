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
        player.animatorCtrl.OnKiss_ANCtrl();
    }
    
    public void OnLick(bool isRight)
    {
        player.animatorCtrl.OnLick_ANCtrl(isRight);
    }
            
    public void OnGrasp(bool isRight)
    {
        player.animatorCtrl.OnGrasp_ANCtrl(isRight);
    }
            
    public void OnSuck(bool isRight)
    {
        player.animatorCtrl.OnSuck_ANCtrl(isRight);
    }
            
    public void OnPinch(bool isRight)
    {
        player.animatorCtrl.OnPinch_ANCtrl(isRight);
    }
    
    public void StopHandMotion(bool isRight)
    {
        player.animatorCtrl.OnStopHandMotion_ANCtrl(isRight);
    }
    
    public void OnMassage() 
    {
        player.animatorCtrl.OnMassage_ANCtrl();
    }
    
    public void OnGrasp_TwoSide()
    {
        player.animatorCtrl.OnGrasp_TwoSide_ANCtrl();
    }
            
    public void OnPinch_TwoSide()
    {
        player.animatorCtrl.OnPinch_TwoSide_ANCtrl();
    }
            
    public void OnInterlockingFingers()
    {
        player.animatorCtrl.OnInterlockingFingers_ANCtrl();
    }
            
    public void OnGrabTheHands()
    {
        player.animatorCtrl.OnGrabTheHands_ANCtrl();
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
