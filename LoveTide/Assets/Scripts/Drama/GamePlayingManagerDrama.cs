using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayingManagerDrama : MonoBehaviour
{
    [SerializeField] public DialogData[] diaData;
    [SerializeField] private PlayerCtrlDrama playerCtrlManager;
    [SerializeField] private SettleManager settleCtrl;
    // Start is called before the first frame update
    private void Awake()
    {
        //PlayerPrefs.SetInt("DramaNumber",1);
        DialogDetected();
        playerCtrlManager.OnStart();
        //AAAAA();
        
    }
    void Start()
    {
        //playerCtrlManager.diaLog = diaData[1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DialogDetected()//01為主線 02外遇 03淫趴 04海灘 05山丘 06公園 07商店街 08上班 09宿舍
    {
        if (PlayerPrefs.GetInt("DramaNumber") == 0)
        {
            PlayerPrefs.SetInt("DramaNumber", 1);
        }
        
        switch (PlayerPrefs.GetInt("DramaNumber"))
        {
            case 1:playerCtrlManager.diaLog = diaData[1]; break;
            case 2:playerCtrlManager.diaLog = diaData[2]; break;
            case 3:playerCtrlManager.diaLog = diaData[3]; break; 
            case 4:playerCtrlManager.diaLog = diaData[4]; break;
            case 5:playerCtrlManager.diaLog = diaData[5]; break;
            case 6:playerCtrlManager.diaLog = diaData[6]; break;
            case 7:playerCtrlManager.diaLog = diaData[7]; break;
            case 8:playerCtrlManager.diaLog = diaData[8]; break;
            case 9:playerCtrlManager.diaLog = diaData[9]; break;
            case 10:playerCtrlManager.diaLog = diaData[10]; break;
        }
        Debug.Log(PlayerPrefs.GetInt("DramaNumber"));
    }

    public async void OnTalkDown()
    {
        settleCtrl.OnSettleDetected();
        await Task.Delay(2000);
        SceneManager.LoadScene("Scenes/GamePlayScene");
    }

    public void AAAAA()
    {
        PlayerPrefs.SetInt("DramaNumber",1);
        PlayerPrefs.SetInt("mainMissionEvent0",0);
        PlayerPrefs.SetString("playerNameData0" , "阿金");
    }
}
