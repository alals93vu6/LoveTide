﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using Task = System.Threading.Tasks.Task;

public class GameManagerTest : MonoBehaviour
{
    [Header("數值")]
    [SerializeField] public NumericalRecords numberCtrl;
    [SerializeField] public BackgroundCtrl background;
    [SerializeField] public TimeManagerTest timer;
    [SerializeField] public TextBoxTestPlaying textBox;
    [SerializeField] public ActorManagerTest actorManager;

    [Header("物件")] 
    [SerializeField] public GameObject[] sceneObject;
    [SerializeField] public GameObject[] interactiveButton;
    [SerializeField] public ActorLocationCtrl actorCtrl;
    [SerializeField] private DialogData[] dialog;

    [Header("狀態")] 
    [SerializeField] public bool isTalk;
    [SerializeField] public bool timePass;
    [SerializeField] public bool isAlone;


    private void Awake()
    {
        numberCtrl.OnStart();
        textBox.OnStart_TextBox(dialog[PlayerPrefs.GetInt("FDS_LV")]);
        actorManager.OnStart(dialog[PlayerPrefs.GetInt("FDS_LV")]);
        CheckActions();
    }

    // Start is called before the first frame update
     void Start()
     {
         OnStart();
     }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
        {
            numberCtrl.GameDataSave();
            Debug.Log("SaveData");
        }
        
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            numberCtrl.GameDataReset();
            Debug.Log("ResetData");
            SceneManager.LoadScene(0);
        }
    }

    public async void OnStart()
    {
        await Task.Delay(80);
        SetClickObject(0);
        CheckActions();
    }

    public void TimePassCheck()
    {
        if (numberCtrl.aTimer >= 10)
        {
            if (timer.vacation)
            {
                Debug.Log("C");
                isTalk = false;
                SetInteractiveObject(false);
                switch (numberCtrl.aTimer)
                {
                    case 10 :OnTalkEvent(64); break;
                    case 11 :OnTalkEvent(64); break;
                    case 12 :OnTalkEvent(62); break;
                    case 13 :OnTalkEvent(63); break;
                }
                timer.vacation = false;
                isAlone = false;
            }
            else
            {
                Debug.Log("D");
                DayPassedEvent(0,0,0);
            }
        }
        else
        {
            if (numberCtrl.aTimer == 7 && !timer.vacation)
            {
                isTalk = false;
                SetInteractiveObject(false);
                OnTalkEvent(61);
                timer.vacation = true;
                //Debug.Log("A");
            }
            else if (numberCtrl.aTimer == 8 && !isAlone)
            {
                isTalk = false;
                SetInteractiveObject(false);
                OnTalkEvent(68);
                isAlone = true;
            }
            else
            {
                SetClickObject(0);
                CheckActions();
                //Debug.Log("B");
            }
        }
        
    }

    public void DayPassedEvent(int fds,int slt, int lst)
    {
        numberCtrl.SetNumerica(fds,slt,lst);
        timer.DetectedDayPassed();
        SetClickObject(0);
        CheckActions();
        numberCtrl.GameDataSave();
        //OnTalkEvent(45);
        //inTextBox = false;
    }

    public void ClickTextBoxEvent()
    {
        if (textBox.isover)
        {
            textBox.DownText();
            //Debug.Log("VAR");
        }
        else
        {
            textBox.NextText();
            //Debug.Log("VARB");
        }
    }

    public void OnTalkEvent(int talkID)
    {
        
        textBox.listSerial = talkID;
        textBox.OnDisplayText();
        SetClickObject(6);
        if (numberCtrl.aTimer == 7 && !timer.vacation) { }
        else if (numberCtrl.aTimer == 10 && timer.vacation) { }
        else
        {
            CheckActions();
        }
        actorCtrl.StayTarget = 0;
        
        if (dialog[PlayerPrefs.GetInt("FDS_LV")].plotOptionsList[textBox.listSerial].notActor)
        {
            SetInteractiveObject(false);
        }
        else
        {
            actorCtrl.gameObject.SetActive(true);
            interactiveButton[0].SetActive(false);
            actorManager.ActorCtrl();
        }
    }
    
    public void TalkDownEvent()
    {
        if (timePass)
        {
            numberCtrl.aTimer++;
            timePass = false;
        }
        textBox.stopLoop = false;
        actorCtrl.StayTarget = 1;
        var apparel = 0;
        if (FindObjectOfType<TimeManagerTest>().vacation){apparel = 1;}else{apparel = 0;}
        actorManager.ChangeFace(apparel,0);
        TimePassCheck();
        if (isTalk)
        {
            SetInteractiveObject(true);
        }
        else
        {
            SetInteractiveObject(false);
        }
    }

    public void ReadyOuting()
    {
        SetInteractiveObject(false);
        interactiveButton[1].SetActive(true);
        background.ChickBackground_Outing(0);
        SetClickObject(-1);
    }

    public void CancelOuting()
    {
        if (FindObjectOfType<PlayerActorTest>().isAlon)
        {
            SetInteractiveObject(false);
        }
        else
        {
            SetInteractiveObject(true);
        }
        interactiveButton[1].SetActive(false);
        background.ChickBackground(ChangeBackGroundNumber(0));
        SetClickObject(0);
    }

    public void SetInteractiveObject(bool isActive)
    {
        interactiveButton[0].SetActive(isActive);
        actorCtrl.gameObject.SetActive(isActive);
    }

    public void SetClickObject(int displayObject)
    {
        for (int i = 0; i < sceneObject.Length; i++)
        {
            sceneObject[i].SetActive(false);
        }

        if (displayObject == 0)
        {
            sceneObject[ChangeClickObjectNumber(0)].SetActive(true);
        }
        else if(displayObject == -1)
        {
            sceneObject[0].SetActive(true);
        }
        else
        {
            sceneObject[displayObject].SetActive(true);
        }
        //CheckActions();
    }
    
    public void CheckActions()
    {
        background.ChickBackground(ChangeBackGroundNumber(0));
        //Debug.Log("ChangeBackground");
    }

    private int ChangeBackGroundNumber(int BackNumber)
    {
        if (timer.vacation)
        {
            
            if (isTalk)
            {
                if (numberCtrl.aTimer == 8)
                {
                    BackNumber += 10;
                }
                else
                {
                    BackNumber += 9;
                }
            }
            else
            {
                if (numberCtrl.aTimer == 8)
                {
                    BackNumber += 5;
                }
                else
                {
                    BackNumber += 4;
                }
            }
        }
        else
        {
            if (isTalk)
            {
                switch (numberCtrl.aTimer)
                {
                    case 1: BackNumber += 6; break;
                    case 2: BackNumber += 6; break;
                    case 3: BackNumber += 7; break;
                    case 4: BackNumber += 7; break;
                    case 5: BackNumber += 8; break;
                    case 6: BackNumber += 8; break;
                    case 7: BackNumber += 8; break;
                    case 8: BackNumber += 10; break;
                    case 9: BackNumber += 9; break;
                } 
            }
            else
            {
                switch (numberCtrl.aTimer)
                {
                    case 1: BackNumber += 1; break;
                    case 2: BackNumber += 1; break;
                    case 3: BackNumber += 2; break;
                    case 4: BackNumber += 2; break;
                    case 5: BackNumber += 3; break;
                    case 6: BackNumber += 3; break;
                    case 7: BackNumber += 4; break;
                    case 8: BackNumber += 5; break;
                    case 9: BackNumber += 4; break;
                }
            }
        }
        
        return BackNumber;
    }

    private int ChangeClickObjectNumber(int objectNumber)
    {

        if (isTalk)
        {
            if (timer.vacation)
            {
                if (numberCtrl.aTimer == 8)
                {
                    objectNumber += 5;
                }
                else
                {
                    objectNumber += 4;
                }
            }
            else
            {
                switch (numberCtrl.aTimer)
                {
                    case 1: objectNumber += 2; break;
                    case 2: objectNumber += 2; break;
                    case 3: objectNumber += 2; break;
                    case 4: objectNumber += 2; break;
                    case 5: objectNumber += 2; break;
                    case 6: objectNumber += 2; break;
                    case 7: objectNumber += 4; break;
                    case 8: objectNumber += 5; break;
                    case 9: objectNumber += 4; break;
                }
            }
        }
        else
        {
            if (timer.vacation)
            {
                objectNumber += 3;
            }
            else
            {
                switch (numberCtrl.aTimer)
                {
                    case 1: objectNumber += 1; break;
                    case 2: objectNumber += 1; break;
                    case 3: objectNumber += 1; break;
                    case 4: objectNumber += 1; break;
                    case 5: objectNumber += 1; break;
                    case 6: objectNumber += 1; break;
                    case 7: objectNumber += 3; break;
                    case 8: objectNumber += 3; break;
                    case 9: objectNumber += 3; break;
                }
            }
        }
        //Debug.Log(objectNumber);
        return objectNumber;
    }

}
