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

    [Header("狀態")] 
    [SerializeField] public bool isTalk;

    

    // Start is called before the first frame update
    void Start()
    {
        textBox.OnStart_TextBox(dialog);
        SetClickObject(0);
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
        if (textBox.isover)
        {
            textBox.DownText();
        }
        else
        {
            textBox.NextText();
        }
    }

    public void SetClickObject(int displayObject)
    {
        for (int i = 0; i < sceneObject.Length; i++)
        {
            sceneObject[i].SetActive(false);
        }

        if (displayObject == 0)
        {
            sceneObject[ChangeNumber(0)].SetActive(true);
        }
        else
        {
            sceneObject[displayObject].SetActive(true);
        }
        
        CheckActions();
    }

    private int ChangeNumber(int objectNumber)
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
