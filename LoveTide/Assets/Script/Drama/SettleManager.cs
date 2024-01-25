using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettleManager : MonoBehaviour
{
    [SerializeField] public NumericalRecords numberCtrl;
    [SerializeField] private int aDayData;
    
    public void OnSettleDetected()
    {
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

    private void MainMissionSettleDetected()
    {
        switch (numberCtrl.mainMission)
        {
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
        }
    }

    private void PartySettleDetected()
    {
        
    }

    private void AlonOutingSettleDetected()
    {
        
    }

    private void BeachSettleDetected()
    {
        
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
