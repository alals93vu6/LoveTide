using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettleManager : MonoBehaviour
{
    [SerializeField] public NumericalRecords numberCtrl;
    [SerializeField] private int targetNumber;

    public void OnSettleDetected()
    {
        targetNumber = FindObjectOfType<EventDetectedManager>().targetDialogNumber;
        switch (PlayerPrefs.GetInt("DramaNumber"))
        {
            case 1: MainMissionSettleDetected(); break;
            case 2: PartySettleDetected(); break;
            case 3: AlonOutingSettleDetected(); break;
            case 4: BeachSettleDetected(); break;
            case 5: HillsSettleDetected(); break;
            case 6: ParkSettleDetected(); break;
            case 7: ShoppingStreetSettleDetected(); break;
            case 8: TavernSettleDetected(); break;
            case 9: DormitoriesSettleDetected(); break;
        }
    }

    private void SetGameDataNumerical(bool isDayPass,int setPassDay, int setTime,int setFDS)
    {
        if (isDayPass)
        {
            numberCtrl.aDay += setPassDay;
            numberCtrl.aWeek += setPassDay;
            numberCtrl.aTimer = 1;
            numberCtrl.friendship += setFDS;
        }
        else
        {
            numberCtrl.aTimer += setTime;
            numberCtrl.friendship += setFDS;
        }
    }
    
    private void MainMissionSettleDetected()
    {
        switch (targetNumber)
        {
            case 0: SetGameDataNumerical(true,7,1,0); break;
            case 1: SetGameDataNumerical(true,1,-10,150); break;
            case 2: SetGameDataNumerical(true,2,-10,200); break;
            case 3: SetGameDataNumerical(true,1,-10,200); numberCtrl.slutty += 50;  break;
            case 4: SetGameDataNumerical(false,0,7,200); numberCtrl.slutty += 50; break;
            case 5: SetGameDataNumerical(false,0,4,200); numberCtrl.slutty += 50; break;
            case 6: SetGameDataNumerical(true,2,-10,350); numberCtrl.slutty += 100; break;
            case 7: SetGameDataNumerical(true,1,-10,300); break;
            case 8: SetGameDataNumerical(true,1,-10,400); numberCtrl.slutty += 150; break;
        }
    }

    private void PartySettleDetected()
    {
        SetGameDataNumerical(true,1,-10,0);
    }

    private void AlonOutingSettleDetected()
    {
        if (targetNumber >= 5)
        {
            if (numberCtrl.aTimer <= 6)
            {
                SetGameDataNumerical(false,0,3,0);
            }
            else
            {
                SetGameDataNumerical(true,1,-10,0);
            }
        }
        else
        {
            SetGameDataNumerical(true,1,-10,0);
        }
    }

    private void BeachSettleDetected()
    {
        if (targetNumber <= 2)
        {
            SetGameDataNumerical(false,0,3,0);
        }
        else if(targetNumber >= 5)
        {
            
        }
        else
        {
            
        }
    }

    private void HillsSettleDetected()
    {
        
    }

    private void ParkSettleDetected()
    {
        
    }

    private void ShoppingStreetSettleDetected()
    {
        
    }

    private void TavernSettleDetected()
    {
        
    }

    private void DormitoriesSettleDetected()
    {
        
    }
}

/*
    case 0:  break;
    case 1:  break;
    case 2:  break;
    case 3:  break;
    case 4:  break;
    case 5:  break;
    case 6:  break;
    case 7:  break;
    case 8:  break;
    case 9:  break;
    */
