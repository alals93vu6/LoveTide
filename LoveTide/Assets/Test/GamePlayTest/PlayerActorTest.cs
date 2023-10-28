using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayerActorTest : MonoBehaviour
{
    [SerializeField]public GameManagerTest gameManager;
    [SerializeField] public DialogDataDetected diaDetected;
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
            case 3: Interactive_InWork(fds,slt,lst); break;//上班調情
            case 4: Interactive_vacation(fds,slt,lst); break;//宿舍調情
            case 5: Interactive_Peeking(fds,slt,lst); break;//偷聽、進房間
            case 6: Interactive_Outing(fds,slt,lst); break;
            case 7: Interactive_Sex(); break;
            case 8: Interactive_Sleep(fds,slt,lst); break;
            case 9: Interactive_OnBack(); break;
            case 10: OnClickTextBox(); break;
        }
    }

    public void Interactive_Speak(int fds,int slt, int lst)
    {
        gameManager.isTalk = true;
        gameManager.OnTalkEvent(0);
    }
    
    public void Interactive_Operate(int fds,int slt, int lst)
    {
        gameManager.timePass = true;
        int talkid = Random.Range(4,7);
        gameManager.OnTalkEvent(talkid);
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_InWork(int fds,int slt, int lst)
    {
        gameManager.timePass = true;
        int talkid = Random.Range(14,17);
        gameManager.OnTalkEvent(0);
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_vacation(int fds,int slt, int lst)
    {
        gameManager.timePass = true;
        int talkid = Random.Range(17,20);
        gameManager.OnTalkEvent(0);
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_Peeking(int fds,int slt, int lst)
    {
        gameManager.timePass = true;
        //int talkid = Random.Range();
        gameManager.OnTalkEvent(0);
    }
    
    public void Interactive_Outing(int fds,int slt, int lst)
    {
        //gameManager.isTalk = false;
        //gameManager.SetClickObject(0);
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
        Debug.Log(talkid);
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

    public void OnClickTextBox()
    {
        gameManager.ClickTextBoxEvent();
    }

}
