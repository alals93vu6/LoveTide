using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerTest : MonoBehaviour
{
    [SerializeField] public NumericalRecords numberCtrl;
    [SerializeField] public BackgroundCtrl background;
    [SerializeField] public TimeManagerTest timer;
    [SerializeField] public TextBoxDrama textBox;

    [SerializeField] private DialogData dialog;
    // Start is called before the first frame update
    void Start()
    {
        textBox.OnStart_TextBox(dialog);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DayPassedEvent(int fds,int slt, int lst)
    {
        numberCtrl.SetNumerica(fds,slt,lst);
        timer.DetectedDayPassed();
    }

    public void CheckActions()
    {
        background.ChickBackground(numberCtrl.aTimer);
    }

    public void OnActions()
    {
        
    }

    public void TimeOffWork()
    {
        
    }

    public void ClickTextBoxEvent()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textBox.isover)
            {
                textBox.DownText();
            }
            else
            {
                textBox.NextText();
                //actorCtrl.ActorCtrl(diaLog.plotOptionsList[0].dialogDataDetails[talkOrder].stayLocation);
                //CGDisplay.DisplayCGChick(diaLog.plotOptionsList[0].dialogDataDetails[talkOrder].switchCGDisplay,diaLog.plotOptionsList[0].dialogDataDetails[talkOrder].switchCGImage);
            }
        }
    }
}
