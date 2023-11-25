using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxTestPlaying : MonoBehaviour
{
    
    [Header("數值")]
    [SerializeField] public Text nameText;
    [SerializeField] public Text showText;
    [SerializeField] public float letterSpeed = 0.02f;
    [SerializeField] public int textNumber = 0;
    [SerializeField] public string[] getTextDate;
    
    [Header("物件")]
    [SerializeField] public DialogData diaLog;
    [SerializeField] public GameObject talkObject;
    [SerializeField] public DialogDataDetected diaLogDetected;
    [SerializeField] public ActorManagerTest actorCtrl;

    [Header("狀態")]
    [SerializeField] public bool isover = true;
    [SerializeField] public bool stopLoop;
    [SerializeField] public int listSerial;
    // Start is called before the first frame update
    void Start()
    {
        //OnStart_TextBox(diaLog);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStart_TextBox(DialogData diadata)
    {
        diaLog = diadata;
        TextDataLoad(listSerial);
        //Debug.Log(diaLog.name);
        //ChickName();
        //StartCoroutine(DisplayTextWithTypingEffect(false)); 
    }

    public void TextDataLoad(int ID)
    {
        //Array.Clear(getTextDate,0,50);
        //Debug.Log(arraySize);
        for (int i = 0; i < diaLog.plotOptionsList[ID].dialogDataDetails.Count; i++)
        {
            getTextDate[i] = diaLog.plotOptionsList[ID].dialogDataDetails[i].sentence;
        }
        //Debug.Log("Onload");
    }
    
    private IEnumerator DisplayTextWithTypingEffect(bool OnWork)
    {
        isover = true;
        if ( getTextDate.Length > textNumber)
        {
            string targetText = getTextDate[textNumber];
            showText.text = "";
            
            if (!OnWork)
            {
                for (int i = 0; i < targetText.Length; i++)
                {
                    if (stopLoop == true)
                    {
                        break;
                    }
                    showText.text += targetText[i];
                    yield return new WaitForSeconds(letterSpeed);
                }
                isover = false;
            }
            else
            {
                showText.text = "";
                showText.text = targetText;
                isover = false;
            }
        }
    }

    public void OnDisplayText()
    {
        TextDataLoad(listSerial);
        textNumber = 0;
        ChickName();
        StartCoroutine(DisplayTextWithTypingEffect(false));
        DisplayTextBox(true);
        //actorCtrl.ActorCtrl();
        
    }

    public void NextText()
    {
        if (textNumber < diaLog.plotOptionsList[listSerial].dialogDataDetails.Count-1)
        {
            stopLoop = false;
            textNumber++;
            ChickName();
            //actorCtrl.TheActor[1].gameObject.SetActive(false);
            actorCtrl.ActorCtrl();
            StartCoroutine(DisplayTextWithTypingEffect(false));
        }
        else
        {
            DisplayTextBox(false);
            FindObjectOfType<GameManagerTest>().TalkDownEvent();
        }

        //Debug.Log(textNumber);
    }
    
    public void DownText()
    {
        stopLoop = true;
        StartCoroutine(DisplayTextWithTypingEffect(true));
    }

    public void DisplayTextBox(bool display)
    {
        talkObject.SetActive(display);
    }

    private void ChickName()
    {
       switch (diaLog.plotOptionsList[listSerial].dialogDataDetails[textNumber].speaker)
        {
            case Speaker.Chorus: nameText.text = " "; break;
            case Speaker.Player: nameText.text = "玩家"; break;
            case Speaker.GirlFriend: nameText.text = "由香"; break;
            case Speaker.BoyFriend: nameText.text = "苦主"; break;
            case Speaker.Steve: nameText.text = "史帝夫"; break;
            case Speaker.PoliceA: nameText.text = "警察A"; break;
            case Speaker.PoliceB: nameText.text = "警察B"; break;
            case Speaker.PassersbyA: nameText.text = "路人A"; break;
            case Speaker.PassersbyB: nameText.text = "路人B"; break;
        }
    }
}
