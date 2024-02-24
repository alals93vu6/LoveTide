using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDetectedManager : MonoBehaviour
{
    [SerializeField] public NumericalRecords numberCtrl;
    [SerializeField] public int targetDialogNumber;
    public int PlayDramaDetected(int returnNumber)//"DramaNumber" 播放編號
    {
        switch (PlayerPrefs.GetInt("DramaNumber"))
        {
            case 1: MainMissionEventDetected(); break;
            case 3: PartyEventDetected(); break;
            case 2: AlonOutingEventDetected(); break;
            case 4: BeachEventDetected(); break;
            case 5: HillsEventDetected(); break;
            case 6: ParkEventDetected(); break;
            case 7: ShoppingStreetEventDetected(); break;
            case 8: TavernEventDetected(); break;
            case 9: DormitoriesEventDetected(); break;
        }
        returnNumber = targetDialogNumber;
        return returnNumber;
    }

    private void MainMissionEventDetected()
    {
        targetDialogNumber = numberCtrl.mainMission;
    }

    private void PartyEventDetected()
    {
        if (numberCtrl.party >= 4)
        {
            targetDialogNumber = 4;
        }
        else
        {
            targetDialogNumber = numberCtrl.mainMission;
        }
    }
    
    private void AlonOutingEventDetected()
    {
        if (numberCtrl.alonOuting >= 5)
        {
            if (numberCtrl.aTimer <= 6)
            {
                targetDialogNumber = Random.Range(5, 8);
            }
            else
            {
                targetDialogNumber = Random.Range(8, 11);
            }
        }
        else
        {
            targetDialogNumber = numberCtrl.mainMission;
        }
    }
    
    private void BeachEventDetected()
    {
        int eventNumber = 0;
        if (numberCtrl.aTimer <= 6)
        {
            eventNumber = 3;
        }
        else
        {
            eventNumber = 4;
        }
        if (numberCtrl.getPropsLevel >= 4 && numberCtrl.beach == 0 && numberCtrl.aTimer <= 4)
        {
            eventNumber = 1;
        }
        if (numberCtrl.getPropsLevel >= 6 && numberCtrl.beach == 1 && numberCtrl.aTimer <= 4)
        {
            eventNumber = 2;
        }
        switch (eventNumber)
        {
            case 1 : targetDialogNumber = 0; break;
            case 2 : targetDialogNumber = 1; break;
            case 3 : targetDialogNumber = Random.Range(2, 5); break;
            case 4 : targetDialogNumber = Random.Range(5, 8); break;
        }
    }
    private void HillsEventDetected()
    {
        int eventNumber = 0;
        if (numberCtrl.aTimer <= 6)
        {
            eventNumber = 3;
        }
        else
        {
            eventNumber = 4;
        }
        if (numberCtrl.getPropsLevel >= 1 && numberCtrl.hills == 0 && numberCtrl.aTimer <= 4 )
        {
            eventNumber = 1;
        }
        if (numberCtrl.getPropsLevel >= 6 && numberCtrl.hills == 1 && numberCtrl.aTimer >= 7)
        {
            eventNumber = 2;
        }
        switch (eventNumber)
        {
            case 1 : targetDialogNumber = 0; break;
            case 2 : targetDialogNumber = 1; break;
            case 3 : targetDialogNumber = Random.Range(2, 5); break;
            case 4 : targetDialogNumber = Random.Range(5, 8); break;
        }
    }
    private void ParkEventDetected()
    {
        int eventNumber = 0;
        if (numberCtrl.aTimer <= 6)
        {
            eventNumber = 3;
        }
        else
        {
            eventNumber = 4;
        }
        if (numberCtrl.lust >= 35 && numberCtrl.park == 0 && numberCtrl.aTimer <= 6)
        {
            eventNumber = 1;
        }
        if (numberCtrl.getPropsLevel >= 5 && numberCtrl.lust >= 35 && numberCtrl.park == 1 && numberCtrl.aTimer >= 7)
        {
            eventNumber = 2;
        }
        switch (eventNumber)
        {
            case 1 : targetDialogNumber = 0; break;
            case 2 : targetDialogNumber = 1; break;
            case 3 : targetDialogNumber = Random.Range(2, 5); break;
            case 4 : targetDialogNumber = Random.Range(5, 8); break;
        }
    }
    
    private void ShoppingStreetEventDetected()
    {
        int eventNumber = 0;
        if (numberCtrl.aTimer <= 6)
        {
            eventNumber = 3;
        }
        else
        {
            eventNumber = 4;
        }
        if (numberCtrl.lust >= 40 && numberCtrl.shoppingStreet == 0 && numberCtrl.aTimer >= 7)
        {
            eventNumber = 1;
        }
        if (numberCtrl.lust >= 40 && numberCtrl.shoppingStreet == 1 && numberCtrl.aTimer >= 7)
        {
            eventNumber = 2;
        }
        switch (eventNumber)
        {
            case 1 : targetDialogNumber = 0; break;
            case 2 : targetDialogNumber = 1; break;
            case 3 : targetDialogNumber = Random.Range(2, 5); break;
            case 4 : targetDialogNumber = Random.Range(5, 8); break;
        }
    }
    
    private void TavernEventDetected()
    {
        targetDialogNumber = numberCtrl.mainMission;
    }
    
    private void DormitoriesEventDetected()
    {
        targetDialogNumber = numberCtrl.mainMission;
    }
}
