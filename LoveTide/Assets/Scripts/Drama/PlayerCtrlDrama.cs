using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCtrlDrama : MonoBehaviour
{
    [Header("物件A")]
    [SerializeField] private TextBoxDrama texBox;
    [SerializeField] private ActorManagerDrama actorCtrl;
    [SerializeField] private CGDisplay CGDisplay;
    [SerializeField] public EventDetectedManager eventDetected;
    
    [Header("物件B")]
    [SerializeField] public DialogData diaLog;
    [SerializeField] private ScenarioChoseSystem scenarioChose;

    [SerializeField] private GameObject texBoxObj;

    [Header("數值")] 
    [SerializeField] private int talkOrder;
    [SerializeField] private float skipInterval;
    // Start is called before the first frame update
    void Start()
    {
        //texBox.targetNumber = eventDetected.PlayDramaDetected(0);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerClick();
        
        if (Input.GetKey(KeyCode.LeftControl) && !texBox.isEnd)
        {
            skipInterval += Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) && !texBox.isEnd)
        {
            skipInterval = 0f;
        }

        if (skipInterval >= 0.07f && !texBox.isEnd)
        {
            PlayerSkip();
            skipInterval = 0f;
        }
    }

    public void OnStart()
    {
        eventDetected.numberCtrl.OnStart();
        texBox.targetNumber = eventDetected.PlayDramaDetected(0);
        texBox.OnStart_TextBox(diaLog);
        actorCtrl.OnStart(diaLog,diaLog.plotOptionsList[eventDetected.PlayDramaDetected(0)].
            dialogDataDetails[talkOrder].stayLocation,eventDetected.PlayDramaDetected(0));
        CGDisplay.OnStart(diaLog,eventDetected.PlayDramaDetected(0));
        Debug.Log(texBox.textNumber);
    }

    private void PlayerClick()
    {
        if (Input.GetMouseButtonDown(0) && !texBox.isWait && !texBox.isEnd)
        {
            SetTextBox(true);
            if (texBox.isover && !texBox.isEnd)
            {
                texBox.DownText();
            }
            else if (!texBox.isEnd)
            {
                texBox.NextText();
                talkOrder = texBox.textNumber;
                actorCtrl.ActorCtrl(diaLog.plotOptionsList[eventDetected.PlayDramaDetected(0)].dialogDataDetails[talkOrder].stayLocation);
                CGDisplay.DisplayCGChick(diaLog.plotOptionsList[eventDetected.PlayDramaDetected(0)].dialogDataDetails[talkOrder].switchCGDisplay,
                    diaLog.plotOptionsList[eventDetected.PlayDramaDetected(0)].dialogDataDetails[talkOrder].switchCGImage);
            }
        }

        if (Input.GetMouseButtonDown(1) && !texBox.isWait)
        {
            DisplayTextBox(0);
        }
    }
    
    

    public void DisplayTextBox(int ctrlNumber)
    {
        switch (ctrlNumber)
        {
            case 0: 
                if (texBoxObj.activeSelf)
                {
                    SetTextBox(false);
                }
                else
                {
                    SetTextBox(true);
                }
                break;
            case 1: SetTextBox(false); break;
            case 2: SetTextBox(true); break;
        }
        
    }

    private void PlayerSkip()
    {
        if (!texBox.isEnd)
        {
            if (!texBox.isWait)
            {
                SetTextBox(true);
                if (texBox.isover)
                {
                    texBox.DownText();
                }
                else
                {
                    texBox.NextText();
                    talkOrder = texBox.textNumber;
                    if (talkOrder < diaLog.plotOptionsList[eventDetected.PlayDramaDetected(0)].dialogDataDetails.Count - 1)
                    {
                        actorCtrl.ActorCtrl(diaLog.plotOptionsList[eventDetected.PlayDramaDetected(0)].dialogDataDetails[talkOrder].stayLocation);
                        CGDisplay.DisplayCGChick(diaLog.plotOptionsList[eventDetected.PlayDramaDetected(0)].dialogDataDetails[talkOrder].switchCGDisplay,
                            diaLog.plotOptionsList[eventDetected.PlayDramaDetected(0)].dialogDataDetails[talkOrder].switchCGImage);
                    }
                } 
            }  
        }
    }

    private void SetTextBox(bool isDisplay)
    {
        texBoxObj.SetActive(isDisplay);
    }
    
    
}
