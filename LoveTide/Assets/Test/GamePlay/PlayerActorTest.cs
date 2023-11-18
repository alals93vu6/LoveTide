using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayerActorTest : MonoBehaviour
{
    [SerializeField]public GameManagerTest gameManager;
    [SerializeField] public DialogDataDetected diaDetected;

    [SerializeField] private bool isAlon;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickActor(int eventNumber,int fds,int slt, int lst)
    {
        switch (eventNumber)
        {
            case 1: Interactive_Speak(fds,slt,lst); break;//開化
            case 2: Interactive_Operate(fds,slt,lst); break;//幫忙工作
            case 3: Interactive_FlirtTalk(fds,slt,lst); break;//情話
            case 4: Interactive_Molest(fds,slt,lst); break;//肢體調情
            case 5: Interactive_Peeking(fds,slt,lst); break;//偷聽、進房間
            case 6: Interactive_Outing(); break;
            case 7: Interactive_Sex(); break;
            case 8: Interactive_Sleep(fds,slt,lst); break;
            case 9: Interactive_OnBack(); break;
            case 10: OnClickTextBox(); break;
            case 11: Interactive_TwoPersonOuting(); break;
        }
    }

    public void Interactive_Speak(int fds,int slt, int lst)
    {
        if (gameManager.numberCtrl.aTimer == 8)
        {
            gameManager.OnTalkEvent(53);
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
        if (gameManager.numberCtrl.aTimer <= 6)
        {
            talkid = Random.Range(4,7);
        }
        else if(gameManager.numberCtrl.aTimer == 8)
        {
            talkid = 50;
        }
        else
        {
            talkid = Random.Range(11,14);
        }

        gameManager.OnTalkEvent(talkid);
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_FlirtTalk(int fds,int slt, int lst)
    {
        gameManager.timePass = true;
        int talkid = 0;
        if (gameManager.numberCtrl.aTimer <= 7)
        {
            talkid = Random.Range(14,17);
        }
        else
        {
            talkid = Random.Range(17,20);
        }
        gameManager.OnTalkEvent(talkid);
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_Molest(int fds,int slt, int lst)
    {
        gameManager.timePass = true;
        gameManager.OnTalkEvent(55);
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
            talkid = 54;
        }
        else
        {
            talkid = 51;
        }
        gameManager.OnTalkEvent(talkid);
    }
    
    public void Interactive_Outing()
    {
        isAlon = true;
        gameManager.ReadyOuting();
    }
    
    public void Interactive_TwoPersonOuting()
    {
        isAlon = false;
        gameManager.ReadyOuting();
    }

    public void CancelOuting()
    {
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
            talkid = Random.Range(1, 4);
        }
        else
        {
            talkid = Random.Range(8, 11);
        }
        //Debug.Log(talkid);
        gameManager.OnTalkEvent(talkid);
        gameManager.numberCtrl.SetNumerica(1,0,0);
        
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
            
        }
        else
        {
            
        }

        Debug.Log("Beach");
    }
    
    public void GotoParks()
    {
        if (isAlon)
        {
            
        }
        else
        {
            
        }
        Debug.Log("Parks");
    }
    
    public void GotoHills()
    {
        if (isAlon)
        {
            
        }
        else
        {
            
        }
        Debug.Log("Hills");
    }
    
    public void GotoStreets()
    {
        if (isAlon)
        {
            
        }
        else
        {
            
        }
        Debug.Log("Streets");
    }

    public void OnClickTextBox()
    {
        gameManager.ClickTextBoxEvent();
    }

}
