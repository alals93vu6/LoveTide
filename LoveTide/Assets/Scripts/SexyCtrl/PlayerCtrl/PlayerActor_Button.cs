using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor_Button : MonoBehaviour
{
    public void StopAllMotion()
    {
        Debug.Log("StopAllMotion");
    }
    
    public void OnKiss()
    { 
        Debug.Log("OnKiss");
    }
    
    public void OnLick(bool isRight)
    { 
        Debug.Log("OnLick");
    }
            
    public void OnGrasp(bool isRight)
    {
        Debug.Log("OnGrasp");
    }
            
    public void OnSuck(bool isRight)
    {
        Debug.Log("OnSuck");
    }
            
    public void OnPinch(bool isRight)
    {
        Debug.Log("OnPinch");
    }
    
    public void StopHandMotion(bool isRight)
    {
        Debug.Log("StopHandMotion");
    }
    
    public void OnMassage() 
    {
        Debug.Log("OnMassage");
    }
    
    public void OnGrasp_TwoSide()
    {
        Debug.Log("OnGrasp_TwoSide");
    }
            
    public void OnPinch_TwoSide()
    {
        Debug.Log("OnPinch_TwoSide");
    }
            
    public void OnInterlockingFingers()
    {
        Debug.Log("OnInterlockingFingers");
    }
            
    public void OnGrabTheHands()
    {
        Debug.Log("OnGrabTheHands");
    }
    
    public void OnStartEnter(bool isHand)
    {
        Debug.Log("OnStartEnter");
    }
            
    public void OnStartQuit(bool isHand)
    {
        Debug.Log("OnStartQuit");
    }
}
