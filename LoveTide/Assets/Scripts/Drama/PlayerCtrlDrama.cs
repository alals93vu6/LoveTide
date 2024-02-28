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
    [SerializeField] private bool isSkip;
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
        
        if (Input.GetKeyDown(KeyCode.LeftControl) && !texBox.isEnd)
        {
            isSkip = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) && !texBox.isEnd)
        {
            isSkip = false;
            skipInterval = 0f;
        }

        if (isSkip  && !texBox.isEnd)
        {
            skipInterval += Time.deltaTime;
        }

        if (skipInterval >= 0.07f)
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
        if (Input.GetMouseButtonDown(0) && !texBox.isWait)
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
                actorCtrl.ActorCtrl(diaLog.plotOptionsList[eventDetected.PlayDramaDetected(0)].dialogDataDetails[talkOrder].stayLocation);
                CGDisplay.DisplayCGChick(diaLog.plotOptionsList[eventDetected.PlayDramaDetected(0)].dialogDataDetails[talkOrder].switchCGDisplay,
                    diaLog.plotOptionsList[eventDetected.PlayDramaDetected(0)].dialogDataDetails[talkOrder].switchCGImage);
            }
        }

        if (Input.GetMouseButtonDown(1) && !texBox.isWait)
        {
            if (texBoxObj.activeSelf)
            {
                SetTextBox(false);
            }
            else
            {
                SetTextBox(true);
            }
        }
    }

    private void PlayerSkip()
    {
        if (texBox.isover)
        {
            texBox.DownText();
        }
        else
        {
            texBox.NextText();
            talkOrder = texBox.textNumber;
            actorCtrl.ActorCtrl(diaLog.plotOptionsList[eventDetected.PlayDramaDetected(0)].dialogDataDetails[talkOrder].stayLocation);
            CGDisplay.DisplayCGChick(diaLog.plotOptionsList[eventDetected.PlayDramaDetected(0)].dialogDataDetails[talkOrder].switchCGDisplay,diaLog.plotOptionsList[0].dialogDataDetails[talkOrder].switchCGImage);
        }
    }

    private void SetTextBox(bool isDisplay)
    {
        texBoxObj.SetActive(isDisplay);
    }
    
    
}
