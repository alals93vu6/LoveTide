using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDetectedManager : MonoBehaviour
{
    [SerializeField] public NumericalRecords numberCtrl;
    [SerializeField] public int targetDialogNumber;
    public void PlayDramaDetected()//"DramaNumber" 播放編號
    {
        switch (PlayerPrefs.GetInt("DramaNumber"))
        {
            case 1: Debug.Log("A"); break;
            case 2: Debug.Log("B"); break;
            case 3: Debug.Log("C"); break;
            case 4: Debug.Log("D"); break;
            case 5: Debug.Log("E"); break;
            case 6: Debug.Log("F"); break;
            case 7: Debug.Log("G"); break;
        }
    }

    private void MainMissionEventDetected()
    {
        switch (numberCtrl.mainMission)
        {
            case 0: Debug.Log("A"); break;
            case 1: Debug.Log("B"); break;
            case 2: Debug.Log("C"); break;
            case 3: Debug.Log("D"); break;
            case 4: Debug.Log("E"); break;
            case 5: Debug.Log("F"); break;
            case 6: Debug.Log("G"); break;
        }
    }

    private void PartyEventDetected()
    {
        if (numberCtrl.party >= 4)
        {
            
        }
        else
        {
            switch (numberCtrl.party)
            {
                case 0: Debug.Log("A"); break;
                case 1: Debug.Log("B"); break;
                case 2: Debug.Log("C"); break;
            }
        }
    }



}
