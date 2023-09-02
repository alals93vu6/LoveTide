using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActorTest : MonoBehaviour
{
    [SerializeField]public GameManagerTest gameManager;
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
            case 6: Interactive_Outing(fds,slt,lst); break;
            case 7: Interactive_Sex(fds,slt,lst); break;
            case 8: Interactive_Sleep(fds,slt,lst); break;
            case 9: OnClickTextBox(); break;
        }
    }

    public void Interactive_Speak(int fds,int slt, int lst)
    {
        
    }
    
    public void Interactive_Operate(int fds,int slt, int lst)
    {
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_InWork(int fds,int slt, int lst)
    {
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_vacation(int fds,int slt, int lst)
    {
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }
    
    public void Interactive_Peeking(int fds,int slt, int lst)
    {
        
    }
    
    public void Interactive_Outing(int fds,int slt, int lst)
    {
        
    }
    
    public void Interactive_Sex(int fds,int slt, int lst)
    {
        
    }
    
    public void Interactive_Sleep(int fds,int slt, int lst)
    {
        gameManager.numberCtrl.SetNumerica(fds,slt,lst);
    }

    public void OnClickTextBox()
    {
        gameManager.ClickTextBoxEvent();
    }

}
