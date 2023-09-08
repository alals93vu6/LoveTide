using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerTest : MonoBehaviour
{
    [Header("數值")]
    [SerializeField] public NumericalRecords numberCtrl;
    [SerializeField] public BackgroundCtrl background;
    [SerializeField] public TimeManagerTest timer;
    [SerializeField] public TextBoxTestPlaying textBox;
    [SerializeField] private DialogData dialog;

    [Header("物件")] 
    [SerializeField] public GameObject[] sceneObject;
    [SerializeField] public GameObject interactiveButton;
    [SerializeField] public ActorLocationCtrl actorCtrl;

    [Header("狀態")] 
    [SerializeField] public bool isTalk;



    // Start is called before the first frame update
    void Start()
    {
        textBox.OnStart_TextBox(dialog);
        SetClickObject(0);
        CheckActions();
        //Debug.Log("fuck my life");
    }

    public void TimePassCheck()
    {
        if (numberCtrl.aTimer >= 10)
        {
            if (timer.vacation)
            {
                OnTalkEvent();
                timer.vacation = false;
                Debug.Log("C");
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
                OnTalkEvent();
                timer.vacation = true;
                Debug.Log("A");
            }
            else
            {
                SetClickObject(0);
                CheckActions();
                Debug.Log("B");
            }
        }
        
        //Todo 互動中超過下班時間會延續互動
    }

    public void DayPassedEvent(int fds,int slt, int lst)
    {
        numberCtrl.SetNumerica(fds,slt,lst);
        timer.DetectedDayPassed();
        SetClickObject(0);
        CheckActions();
        OnTalkEvent();
        //inTextBox = false;
    }

    public void ClickTextBoxEvent()
    {
        if (textBox.isover)
        {
            textBox.DownText();
        }
        else
        {
            textBox.NextText();
        }
    }

    public void OnTalkEvent()
    {
        textBox.OnDisplayText();
        SetClickObject(6);
        if (numberCtrl.aTimer == 7 && !timer.vacation)
        {
            Debug.Log("OffWork");
        }
        else if (numberCtrl.aTimer == 10 && timer.vacation)
        {
            Debug.Log("TimeToSleep");
        }
        else
        {
            CheckActions();
        }
        actorCtrl.gameObject.SetActive(true);
        interactiveButton.SetActive(false);
        actorCtrl.StayTarget = 0;
    }
    
    public void TalkDownEvent()
    {
        TimePassCheck();
        actorCtrl.StayTarget = 1;
        if (isTalk)
        {
            SetInteractiveObject(true);
        }
        else
        {
            SetInteractiveObject(false);
        }
    }
    

    public void SetInteractiveObject(bool isActive)
    {
        interactiveButton.SetActive(isActive);
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
        if (isTalk)
        {
            if (timer.vacation)
            {
                if (numberCtrl.aTimer == 8)
                {
                    BackNumber += 8;
                }
                else
                {
                    BackNumber += 7;
                }
            }
            else
            {
                switch (numberCtrl.aTimer)
                {
                    case 1: BackNumber += 6; break;
                    case 2: BackNumber += 6; break;
                    case 3: BackNumber += 6; break;
                    case 4: BackNumber += 6; break;
                    case 5: BackNumber += 6; break;
                    case 6: BackNumber += 6; break;
                    case 7: BackNumber += 7; break;
                    case 8: BackNumber += 8; break;
                    case 9: BackNumber += 7; break;
                }
            }
        }
        else
        {
            if (timer.vacation)
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
