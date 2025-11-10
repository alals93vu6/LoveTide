using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ActorManagerTest : MonoBehaviour
{
    public YukaSpineController TheActor;
    [SerializeField] private int idleActor = 0;
    // Start is called before the first frame update

    public void ActorCtrl(bool vacation,string action)
    {
        TheActor.ChangeYukaApparel(vacation, action);
    }

    public void MoveActorLocation(int targetLocation)
    {
        TheActor.GetComponent<ActorLocationCtrl>().StayTarget = targetLocation;
    }
}
