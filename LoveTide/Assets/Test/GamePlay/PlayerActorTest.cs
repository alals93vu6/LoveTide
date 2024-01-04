using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class PlayerActorTest : MonoBehaviour
{
    [SerializeField]public GameManagerTest gameManager;
    [SerializeField] public DialogDataDetected diaDetected;

    [SerializeField] public bool isAlon;
    [SerializeField] private bool isSkip;
    [SerializeField] private bool isOuting;
    [SerializeField] private float skipInterval;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isSkip = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isSkip = false;
            skipInterval = 0f;
        }

        if (isSkip)
        {
            skipInterval += Time.deltaTime;
        }

        if (!isOuting)
        {
            if (skipInterval >= 0.07f)
            {
                OnClickTextBox();
                skipInterval = 0f;
            }
        }
    }

    public void OnClickActor(int eventNumber,int fds,int slt, int lst)
    {
        switch (eventNumber)
        {
            case 1: Interactive_Speak(fds,slt,lst); break;//開化
            case 2: Interactive_Operate(fds,slt,lst); break;//幫忙工作
            case 3: Interactive_FlirtTalk(fds,slt,lst); break;//情話
            case 5: Interactive_Peeking(fds,slt,lst); break;//偷聽、進房間
            case 6: Interactive_Outing(); break;
            case 7: Interactive_Sex(); break;
            case 8: Interactive_Sleep(fds,slt,lst); break;
            case 9: Interactive_OnBack(); break;
            case 10: OnClickTextBox(); break;
            case 11: Interactive_TwoPersonOuting(); break;
            case 12: Interactive_MolestA(fds,slt,lst); break;//奶ㄗ
            case 13: Interactive_MolestB(fds,slt,lst); break;//屁股
            case 14: Interactive_MolestC(fds,slt,lst); break;//該逼
        }
    }

    public void Interactive_Speak(int fds,int slt, int lst)
    {
        if (gameManager.numberCtrl.aTimer == 8)
        {
            gameManager.OnTalkEvent(68);
        }
        else
        {
            gameManager.isTalk = true;
            gameManager.OnTalkEvent(0);
        }
    }
    
    public void Interactive_Operate(int fds,int slt, int lst)
    {
        gameManager.timePass = true;
        int talkid = 0;
        if (!gameManager.timer.vacation)
        {
            talkid = Random.Range(23,28);
        }
        else if(gameManager.numberCtrl.aTimer == 8)
        {
            talkid = 67;
        }
        else
        {
            talkid = Random.Range(50,55);
        }

        gameManager.OnTalkEvent(talkid);
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_FlirtTalk(int fds,int slt, int lst)
    {
        gameManager.timePass = true;
        int talkid = 0;
        if (gameManager.timer.vacation == false)
        {
            talkid = Random.Range(6,11);
        }
        else
        {
            talkid = Random.Range(33,37);
        }
        gameManager.OnTalkEvent(talkid);
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_MolestA(int fds,int slt, int lst)
    {
        gameManager.timePass = true;
        int talkid = 0;
        if (gameManager.timer.vacation == false)
        {
            talkid = Random.Range(11,14);
        }
        else
        {
            talkid = Random.Range(38,41);
        }
        gameManager.OnTalkEvent(talkid);
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_MolestB(int fds,int slt, int lst)
    {
        gameManager.timePass = true;
        int talkid = 0;
        if (gameManager.timer.vacation == false)
        {
            talkid = Random.Range(14,17);
        }
        else
        {
            talkid = Random.Range(44,47);
        }
        gameManager.OnTalkEvent(talkid);
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_MolestC(int fds,int slt, int lst)
    {
        gameManager.timePass = true;
        int talkid = 0;
        if (gameManager.timer.vacation == false)
        {
            talkid = Random.Range(17,20);
        }
        else
        {
            talkid = Random.Range(41,44);
        }
        gameManager.OnTalkEvent(talkid);
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_Peeking(int fds,int slt, int lst)
    {
        //gameManager.timePass = true;
        //int talkid = Random.Range();
        gameManager.SetInteractiveObject(false);
        int talkid = 0;
        if (gameManager.numberCtrl.aTimer != 8)
        {
            talkid = 65;
        }
        else
        {
            talkid = 66;
        }
        gameManager.OnTalkEvent(talkid);
    }
    
    public void Interactive_Outing()
    {
        isAlon = true;
        isOuting = true;
        gameManager.ReadyOuting();
    }
    
    public void Interactive_TwoPersonOuting()
    {
        isAlon = false;
        isOuting = true;
        gameManager.ReadyOuting();
    }

    public void CancelOuting()
    {
        isOuting = false;
        gameManager.CancelOuting();
    }

    public void Interactive_Sex()
    {
        Debug.Log("Sex");
    }
    
    public void Interactive_Talk()
    {
        gameManager.timePass = true;
        int talkid;
        if (!gameManager.timer.vacation)
        {
            talkid = Random.Range(1, 6);
        }
        else
        {
            talkid = Random.Range(28, 33);
        }
        switch (PlayerPrefs.GetInt("FDS_LV"))
        {
            case 0:
                gameManager.numberCtrl.SetNumerica(2,0,0);
                break;
            case 1:
                gameManager.numberCtrl.SetNumerica(2,0,0);
                break;
            case 2:
                gameManager.numberCtrl.SetNumerica(2,0,0);
                break;
            case 3:
                gameManager.numberCtrl.SetNumerica(3,0,0);
                break;
            case 4:
                gameManager.numberCtrl.SetNumerica(3,0,0);
                break;
        }
        gameManager.OnTalkEvent(talkid);
    }
    
    public void Interactive_Sleep(int fds,int slt, int lst)
    {
        
        if (gameManager.numberCtrl.aTimer <= 6)
        {
            gameManager.numberCtrl.aTimer = 12;
        }
        else if(gameManager.numberCtrl.aTimer >= 7 || gameManager.numberCtrl.aTimer <= 9)
        {
            gameManager.numberCtrl.aTimer = 13;
        }
        else
        {
            gameManager.numberCtrl.aTimer = 11;
        }
        gameManager.timePass = true;
        gameManager.TimePassCheck();
        //gameManager.DayPassedEvent(fds,slt,lst);
    }

    public void Interactive_OnBack()
    {
        gameManager.isTalk = false;
        gameManager.actorCtrl.gameObject.SetActive(false);
        gameManager.SetClickObject(0);
        gameManager.CheckActions();
        gameManager.SetInteractiveObject(false);
    }

    public void Interactive_Travel()
    {
        Debug.Log("Travel");
    }

    public void GotoBeach()
    {
        if (isAlon)
        {
            //gameManager.numberCtrl.party++;
            Debug.Log("銀趴");
        }
        else
        {
            Debug.Log("海灘");
            //gameManager.numberCtrl.SetNumerica(5,0,0);
        }/*
        gameManager.numberCtrl.GameDataSave();
        SceneManager.LoadScene("DramaScene");
        Debug.Log("Beach");*/
    }
    
    public void GotoParks()
    {
        if (isAlon)
        {
            Debug.Log("支線+1");
        }
        else
        {
            Debug.Log("Parks");
        }
        
    }
    
    public void GotoHills()
    {
        if (isAlon)
        {
            Debug.Log("支線+1");
        }
        else
        {
            Debug.Log("Hills");
        }
        
    }
    
    public void GotoStreets()
    {
        if (isAlon)
        {
            Debug.Log("支線+1");
        }
        else
        {
            Debug.Log("Streets");
        }
    }

    public void OnClickTextBox()
    {
        gameManager.ClickTextBoxEvent();
    }

}
