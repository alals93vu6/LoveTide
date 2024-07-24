using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Fishing_SettleState : IState
{
    public async void OnEnterState(object action)
    {
        var manager = (Fishingmanager)action;
        manager.sliderQTE.OnQuitQTE();
        manager.QTEon = false;
        manager.fishStamina.staminaAnimator.Play("Stamina_QuitQTE");
        manager.fishActor.Play("FishActorTest_GetFish");
        await Task.Delay(2000);
        manager.ChangeState(new Fishing_IdleState());
    }

    public void OnStayState(object action)
    {
        var manager = (Fishingmanager)action;
        manager.fishStamina.staminaImage.color = Color.Lerp(manager.fishStamina.staminaImage.color,manager.fishStamina.staminaColor[0],2f);
    }

    public void OnExitState(object action)
    {
        var manager = (Fishingmanager)action;
        manager.fishActor.Play("FishActorTest_backIdle");
    }
}
