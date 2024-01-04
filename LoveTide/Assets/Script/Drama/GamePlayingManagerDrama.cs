using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayingManagerDrama : MonoBehaviour
{
    [SerializeField] public DialogData[] diaData;
    [SerializeField] private NumericalRecords numberCtrl;
    [SerializeField] private TextBoxDrama textBoxManager;
    // Start is called before the first frame update
    void Start()
    {
        numberCtrl.OnStart();
        DialogDetected();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DialogDetected()
    {
        switch (PlayerPrefs.GetInt("DramaNumber"))
        {
            case 1:textBoxManager.diaLog = diaData[MainMissionDetected(0)]; break;
            case 2:textBoxManager.diaLog = diaData[ExtramaritalAffairEventDetected(0)]; break;
            case 3:textBoxManager.diaLog = diaData[PartyEventDetected(0)]; break; case 4:textBoxManager.diaLog = diaData[OutingEvent_Beach(0)]; break;
            case 5:textBoxManager.diaLog = diaData[OutingEvent_Hills(0)]; break;
            case 6:textBoxManager.diaLog = diaData[OutingEvent_Parks(0)]; break;
            case 7:textBoxManager.diaLog = diaData[OutingEvent_Restaurant(0)]; break;
        }
    }

    private int MainMissionDetected(int detectedNumber)
    {
        return detectedNumber;
    }

    private int ExtramaritalAffairEventDetected(int detectedNumber)
    {
        return detectedNumber;
    }

    private int PartyEventDetected(int detectedNumber)
    {
        return detectedNumber;
    }

    private int OutingEvent_Beach(int detectedNumber)
    {
        return detectedNumber;
    }
    
    private int OutingEvent_Hills(int detectedNumber)
    {
        return detectedNumber;
    }
    
    private int OutingEvent_Parks(int detectedNumber)
    {
        return detectedNumber;
    }
    
    private int OutingEvent_Restaurant(int detectedNumber)
    {
        return detectedNumber;
    }

    //01為主線 02外遇 03淫趴 04海灘 05山丘 06公園 07吃飯
}
