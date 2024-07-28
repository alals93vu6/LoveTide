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
        manager.levelSet.levelSetAnimator.Play("LevelSet_QTE_QuitQTE");
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
        manager.levelSet.levelSetAnimator.Play("levelSet_Ready_InQTE");
        manager.fishActor.Play("FishActorTest_StartFishing");
        await Task.Delay(2600);
        manager.sliderQTE.OnStartQTE();
        manager.fishStamina.staminaAnimator.Play("Stamina_StartQTE");
        manager.nowStamina = manager.maxStamina;
        await Task.Delay(1200);
        
        manager.QTEon = true;
    }
}