using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Fishing_IdleState : IState
{
    public void OnEnterState(object action)
    {
        var manager = (Fishingmanager)action;
        manager.nowStamina = manager.maxStamina;
    }

    public void OnStayState(object action)
    {
        var manager = (Fishingmanager)action;
        if (Input.GetKeyDown(KeyCode.E))
        {
            manager.ChangeState(new Fishing_StruggleState());
        }
    }

    public async void OnExitState(object action)
    {
        var manager = (Fishingmanager)action;
        manager.sliderQTE.OnStartQTE();
        manager.fishStamina.staminaAnimator.Play("Stamina_StartQTE");
        manager.nowStamina = manager.maxStamina;

        await Task.Delay(1000);
        
        manager.QTEon = true;
    }
}