using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            case 1: Interactive_Speak(fds,slt,lst); break;
            case 2: Interactive_Operate(fds,slt,lst); break;
            case 3: Interactive_InWork(fds,slt,lst); break;
            case 4: Interactive_vacation(fds,slt,lst); break;
            case 5: Interactive_Peeking(fds,slt,lst); break;
            case 6: Interactive_Outing(fds,slt,lst); Debug.Log("Outing"); break;
            case 7: Interactive_Sex(); break;
            case 8: Interactive_Sleep(fds,slt,lst); Debug.Log("sleep"); break;
            case 9: Interactive_OnBack(); Debug.Log("quit"); break;
            case 10: OnClickTextBox(); break;
        }
    }

    public void Interactive_Speak(int fds,int slt, int lst)
    {
        gameManager.isTalk = true;
        gameManager.OnTalkEvent();
    }
    
    public void Interactive_Operate(int fds,int slt, int lst)
    {
        gameManager.numberCtrl.aTimer++;
        gameManager.OnTalkEvent();
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_InWork(int fds,int slt, int lst)
    {
        gameManager.numberCtrl.aTimer++;
        gameManager.OnTalkEvent();
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_vacation(int fds,int slt, int lst)
    {
        gameManager.numberCtrl.aTimer++;
        gameManager.OnTalkEvent();
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_Peeking(int fds,int slt, int lst)
    {
        gameManager.numberCtrl.aTimer++;
        gameManager.OnTalkEvent();
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
        gameManager.numberCtrl.aTimer++;
        gameManager.OnTalkEvent();
        gameManager.numberCtrl.SetNumerica(1,0,0);
    }
    
    public void Interactive_Sleep(int fds,int slt, int lst)
    {
        gameManager.numberCtrl.aTimer = 10;
        //gameManager.OnTalkEvent();
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
