using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxDrama : MonoBehaviour
{
    [Header("數值")]
    [SerializeField] public Text nameText;
    [SerializeField] public Text showText;
    [SerializeField] public float letterSpeed = 0.02f;
    [SerializeField] public int textNumber = 0;
    [SerializeField] public string[] getTextDate;
    [SerializeField] public int targetNumber;
    
    [Header("物件")]
    [SerializeField] public DialogData diaLog;
    [SerializeField] private DirtyTrickCtrl dirtyTrick;

    [Header("狀態")]
    [SerializeField] public bool isover = true;
    [SerializeField] public bool stopLoop;
    [SerializeField] public bool isWait;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStart_TextBox(DialogData diadata)
    {
        diaLog = diadata;
        TextDataLoad();
        ChickName();
        StartCoroutine(DisplayTextWithTypingEffect(false)); 
    }
    
    private void TextDataLoad()
    {
        var arraySize = diaLog.plotOptionsList[targetNumber].dialogDataDetails.Count;
        //Debug.Log(arraySize);
        for (int i = 0; i < diaLog.plotOptionsList[targetNumber].dialogDataDetails.Count; i++)
        {
            getTextDate[i] = diaLog.plotOptionsList[targetNumber].dialogDataDetails[i].sentence;
        }
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
    
    public async void NextText()
    {
        if (textNumber < diaLog.plotOptionsList[targetNumber].dialogDataDetails.Count - 1)
        {
            if (!diaLog.plotOptionsList[targetNumber].dialogDataDetails[textNumber].needTransition)
            {
                stopLoop = false;
                textNumber++;
                ChickName();
                StartCoroutine(DisplayTextWithTypingEffect(false));
            }
            else
            {
                dirtyTrick.OnChangeScenes();
                stopLoop = false;
                textNumber++;
                isWait = true;
                await Task.Delay(500);
                isWait = false;
                ChickName();
                StartCoroutine(DisplayTextWithTypingEffect(false));
            }
            
        }
        else
        {
            TalkOver();
        }
    }
    
    public void DownText()
    {
        stopLoop = true;
        StartCoroutine(DisplayTextWithTypingEffect(true));
    }

    public void TalkOver()
    {
        dirtyTrick.OnExitGamePlayScenes();
    }

    private void ChickName()
    {
       switch (diaLog.plotOptionsList[targetNumber].dialogDataDetails[textNumber].speaker)
        {
            case Speaker.Chorus: nameText.text = ""; break;
            case Speaker.Player: nameText.text = "玩家"; break;
            case Speaker.GirlFriend: nameText.text = "由香"; break;
            case Speaker.BoyFriend: nameText.text = "苦主"; break;
            case Speaker.Steve: nameText.text = "史帝夫"; break;
            case Speaker.PoliceA: nameText.text = "警察A"; break;
            case Speaker.PoliceB: nameText.text = "警察B"; break;
            case Speaker.PassersbyA: nameText.text = "路人A"; break;
            case Speaker.PassersbyB: nameText.text = "路人B"; break;
            case Speaker.TavernBoss: nameText.text = "老闆"; break;
        }
    }
}
