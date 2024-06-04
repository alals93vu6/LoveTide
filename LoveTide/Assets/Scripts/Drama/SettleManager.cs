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
            case 3: PartySettleDetected(); break;
            case 2: AlonOutingSettleDetected(); break;
            case 4: BeachSettleDetected(); break;
            case 5: HillsSettleDetected(); break;
            case 6: ParkSettleDetected(); break;
            case 7: ShoppingStreetSettleDetected(); break;
            case 8: TavernSettleDetected(); break;
            case 9: DormitoriesSettleDetected(); break;
        }
        numberCtrl.GameDataSave();
        //Debug.Log("TalkDown");
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
            case 0: SetGameDataNumerical(true,1,1,22);
                numberCtrl.mainMission++; break;
            case 1: SetGameDataNumerical(true,1,-10,150);
                numberCtrl.mainMission++;break;
            case 2: SetGameDataNumerical(true,2,-10,200);
                numberCtrl.mainMission++;break;
            case 3: SetGameDataNumerical(true,1,-10,200);
                numberCtrl.mainMission++; numberCtrl.slutty += 50;  break;
            case 4: SetGameDataNumerical(false,0,7,200);
                numberCtrl.mainMission++; numberCtrl.slutty += 50; break;
            case 5: SetGameDataNumerical(false,0,4,200);
                numberCtrl.mainMission++; numberCtrl.slutty += 50; break;
            case 6: SetGameDataNumerical(true,2,-10,350);
                numberCtrl.mainMission++; numberCtrl.slutty += 100; break;
            case 7: SetGameDataNumerical(true,1,-10,300);
                numberCtrl.mainMission++;break;
            case 8: SetGameDataNumerical(true,1,-10,400);
                numberCtrl.mainMission++; numberCtrl.slutty += 150; break;
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
            numberCtrl.alonOuting++;
        }
    }

    private void BeachSettleDetected()
    {
        if (targetNumber <= 1)
        {
            SetGameDataNumerical(false, 0, 3, 150); numberCtrl.slutty += 100;
            numberCtrl.beach++;
        }
        else if(targetNumber >= 5)
        {
            SetGameDataNumerical(true, 1, -10, 8);
        }
        else
        {
            SetGameDataNumerical(false, 0, 3, 8);
            numberCtrl.party++;
        }
    }

    private void HillsSettleDetected()
    {
        if (targetNumber <= 1)
        {
            SetGameDataNumerical(false, 0, 3, 150); numberCtrl.slutty += 100;
            numberCtrl.hills++;
        }
        else if(targetNumber >= 5)
        {
            SetGameDataNumerical(true, 1, -10, 8);
        }
        else
        {
            SetGameDataNumerical(false, 0, 3, 8);
        }
    }

    private void ParkSettleDetected()
    {
        if (targetNumber <= 1)
        {
            SetGameDataNumerical(false, 0, 3, 150); numberCtrl.slutty += 100;
            numberCtrl.park++;
        }
        else if(targetNumber >= 5)
        {
            SetGameDataNumerical(true, 1, -10, 8);
        }
        else
        {
            SetGameDataNumerical(false, 0, 3, 8);
        }
    }

    private void ShoppingStreetSettleDetected()
    {
        if (targetNumber <= 1)
        {
            SetGameDataNumerical(false, 0, 3, 150); numberCtrl.slutty += 100;
            numberCtrl.shoppingStreet++;
        }
        else if(targetNumber >= 5)
        {
            SetGameDataNumerical(true, 1, -10, 8);
        }
        else
        {
            SetGameDataNumerical(false, 0, 3, 8);
        }
    }

    private void TavernSettleDetected()
    {
        SetGameDataNumerical(false, 0, 3, 150); numberCtrl.slutty += 100;
        numberCtrl.tavern++;
    }

    private void DormitoriesSettleDetected()
    {
        SetGameDataNumerical(true, 1, -10, 150); numberCtrl.slutty += 100;
        numberCtrl.dormitories++;
    }
}
