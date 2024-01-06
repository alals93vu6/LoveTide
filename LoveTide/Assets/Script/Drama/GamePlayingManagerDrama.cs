using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayingManagerDrama : MonoBehaviour
{
    [SerializeField] public DialogData[] diaData;
    [SerializeField] private NumericalRecords numberCtrl;
    [SerializeField] private PlayerCtrlDrama playerCtrlManager;
    // Start is called before the first frame update
    private void Awake()
    {
        
    }
    void Start()
    {
        numberCtrl.OnStart();
        DialogDetected();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DialogDetected()//01為主線 02外遇 03淫趴 04海灘 05山丘 06公園 07吃飯
    {
        switch (PlayerPrefs.GetInt("DramaNumber"))
        {
            case 1:playerCtrlManager.diaLog = diaData[1]; break;
            case 2:playerCtrlManager.diaLog = diaData[2]; break;
            case 3:playerCtrlManager.diaLog = diaData[3]; break; 
            case 4:playerCtrlManager.diaLog = diaData[4]; break;
            case 5:playerCtrlManager.diaLog = diaData[5]; break;
            case 6:playerCtrlManager.diaLog = diaData[6]; break;
            case 7:playerCtrlManager.diaLog = diaData[7]; break;
        }
    }
    
}
