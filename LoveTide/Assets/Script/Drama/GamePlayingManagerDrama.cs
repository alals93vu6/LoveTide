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
            case 1:MainMissionDetected(); break;
            case 2:ExtramaritalAffairEventDetected(); break;
            case 3:PartyEventDetected(); break;
            case 4:OutingEvent_Beach(); break;
            case 5:OutingEvent_Hills(); break;
            case 6:OutingEvent_Parks(); break;
            case 7:OutingEvent_Restaurant(); break;
        }
        textBoxManager.diaLog = diaData[0];
    }

    private void MainMissionDetected()
    {
        
    }

    private void ExtramaritalAffairEventDetected()
    {
        
    }

    private void PartyEventDetected()
    {
        
    }

    private void OutingEvent_Beach()
    {
        
    }
    
    private void OutingEvent_Hills()
    {
        
    }
    
    private void OutingEvent_Parks()
    {
        
    }
    
    private void OutingEvent_Restaurant()
    {
        
    }

    //01為主線 02外遇 03淫趴 04海灘 05山丘 06公園 07吃飯
}
