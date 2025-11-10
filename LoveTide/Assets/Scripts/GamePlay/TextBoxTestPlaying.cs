using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxTestPlaying : MonoBehaviour
{
    
    [Header("數值")]
    [SerializeField] public Text nameText;
    [SerializeField] public Text showText;
    [SerializeField] public float letterSpeed = 0.02f;
    [SerializeField] public int textNumber = 0;
    [SerializeField] public List<GameTextBoxDiaData> gameTextBoxDiaDatas;

    [Header("物件")]
    [SerializeField] public GameObject talkObject;
    [SerializeField] public NumericalRecords numericalData;
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

    public void OnStart_TextBox()
    {
         
    }

    public void TextDataLoad(int ID,List<GameDiaData> diaDatas)
    {       
        gameTextBoxDiaDatas = diaDatas
            .Where(data => data.EventIndex == ID)
            .Select(data => new GameTextBoxDiaData
            {
                DailogIndex = data.DailogIndex,
                ActorName = data.ActorName,
                Dailog = data.Dailog.Replace("playername", numericalData.playerName)
            })
            .ToList();
        if (ID == 67)
        {
            gameTextBoxDiaDatas[0].Dailog = NumericalLog("A");
            gameTextBoxDiaDatas[1].Dailog = ConditionLog("B");
        }
        /*Debug.Log($"[TextDataLoad] 共載入 {gameTextBoxDiaDatas.Count} 筆資料：\n" +
        string.Join("\n", gameTextBoxDiaDatas.Select(x => $"{x.ActorName}: {x.Dailog}")));*/
    }
    
    private IEnumerator DisplayTextWithTypingEffect(bool OnWork)
    {
        isover = true;
        if (gameTextBoxDiaDatas.Count > textNumber)
        {
            string targetText = gameTextBoxDiaDatas[textNumber].Dailog;
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

    public void OnDisplayText(List<GameDiaData> diaDatas)
    {
        TextDataLoad(listSerial, diaDatas);
        textNumber = 0;
        OnChickName();
        StartCoroutine(DisplayTextWithTypingEffect(false));
        DisplayTextBox(true);
        //actorCtrl.ActorCtrl();
    }

    public void NextText()
    {
        if (textNumber < gameTextBoxDiaDatas.Count-1)
        {
            stopLoop = false;
            textNumber++;
            OnChickName();
            //actorCtrl.TheActor[1].gameObject.SetActive(false);
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

    private void OnChickName()
    {
        //Debug.Log($"NameDetected:{gameTextBoxDiaDatas[textNumber].ActorName}");
        switch (PlayerPrefs.GetInt("LanguageSet"))
        {
            case 0: ChickName_EN(); break;
            case 1: ChickName_JP(); break;
            case 2: ChickName_TW(); break;
            case 3: ChickName_CN(); break;
        }
    }

    private void ChickName_TW()
    {
        switch (gameTextBoxDiaDatas[textNumber].ActorName)
        {
            case "Narrator": nameText.text = " "; break;
            case "Player": nameText.text = PlayerPrefs.GetString("playerNameData" + PlayerPrefs.GetInt("GameDataNumber").ToString()); ; break;
            case "Yuka": nameText.text = "由香"; break;
            case "Yuka-Worker": nameText.text = "由香"; break;
            case "Yuka-Home": nameText.text = "由香"; break;
            case "guest": nameText.text = "兄弟"; break;
            default:
                nameText.text = "???";
                break;
        }
    }

    private void ChickName_CN()
    {
        switch (gameTextBoxDiaDatas[textNumber].ActorName)
        {
            case "Narrator": nameText.text = " "; break;
            case "Player": nameText.text = PlayerPrefs.GetString("playerNameData" + PlayerPrefs.GetInt("GameDataNumber").ToString()); ; break;
            case "Yuka": nameText.text = "由香"; break;
            case "Yuka-Worker": nameText.text = "由香"; break;
            case "Yuka-Home": nameText.text = "由香"; break;
            case "guest": nameText.text = "兄弟"; break;
            default:
                nameText.text = "???";
                break;
        }
    }

    private void ChickName_EN()
    {
        switch (gameTextBoxDiaDatas[textNumber].ActorName)
        {
            case "Narrator": nameText.text = " "; break;
            case "Player": nameText.text = PlayerPrefs.GetString("playerNameData" + PlayerPrefs.GetInt("GameDataNumber").ToString()); ; break;
            case "Yuka": nameText.text = "Yuka"; break;
            case "Yuka-Worker": nameText.text = "Yuka"; break;
            case "Yuka-Home": nameText.text = "由香"; break;
            case "guest": nameText.text = "Bro"; break;
            default:
                nameText.text = "???";
                break;
        }
    }

    private void ChickName_JP()
    {
        switch (gameTextBoxDiaDatas[textNumber].ActorName)
        {
            case "Narrator": nameText.text = " "; break;
            case "Player": nameText.text = PlayerPrefs.GetString("playerNameData" + PlayerPrefs.GetInt("GameDataNumber").ToString()); ; break;
            case "Yuka": nameText.text = "由香"; break;
            case "Yuka-Worker": nameText.text = "由香"; break;
            case "Yuka-Home": nameText.text = "由香"; break;
            case "guest": nameText.text = "ダチ"; break;
            default:
                nameText.text = "???";
                break;
        }
    }
    private string NumericalLog(string numericalText)
    {
        string periodText = "LogText";
        switch (numericalData.aTimer)
        {
            case 1 : periodText = "     是個涼爽的早晨，而今天也正要開始"; break;
            case 2 : periodText = "     是個涼爽的早晨，而今天也正要開始"; break;
            case 3 : periodText = "     默默地到了中午，太陽非常大"; break;
            case 4 : periodText = "     默默地到了中午，太陽非常大"; break;
            case 5 : periodText = "     黃昏的陽光非常明顯，就快下班了"; break;
            case 6 : periodText = "     黃昏的陽光非常明顯，就快下班了"; break;
            case 7 : periodText = "     目前是寧靜的夜晚，很適合休息"; break;
            case 8 : periodText = "     目前是寧靜的夜晚，很適合休息"; break;
            case 9 : periodText = "     快要進入半夜，時間也不晚了"; break;
        }
        numericalText = "目前已度過： " + numericalData.aDay + " 天  今日為星期  " + numericalData.aWeek + periodText + "\n"
                        + "好感度為： " + numericalData.friendship + "   淫亂度為： " + numericalData.slutty;
        return numericalText;
    }

    private string ConditionLog(string conditionText) //PlayerPrefs.GetInt("FDS_LV")
    {
        string friendshipText = "A";
        switch (PlayerPrefs.GetInt("FDS_LV"))
        {
            case 0 : friendshipText = "總給人一種似乎有所保留的感覺，"; break;
            case 1 : friendshipText = "雖然和以往比起來明顯感受到親近了不少，但多少還是會下意識的避嫌，"; break;
            case 2 : friendshipText = "現在關係非常神奇，曖昧不清的感覺也總是不斷出現，"; break;
            case 3 : friendshipText = "彼此明確成為了不可外傳的關係，"; break;
            case 4 : friendshipText = "沉淪於背德感之中，屬實是無藥可救的一段關係，"; break;
        }

        string lustText = "B";
        switch (LustDetected(0))
        {
            case 0 : lustText = "不過由香那活潑開朗的模樣依舊令人感到很有活力"; break;
            case 1 : lustText = "明顯地感覺到由香似乎會嘗試製造更多的肢體接觸"; break;
            case 2 : lustText = "兩人平時看似平平無奇的互動，但由香總是會偷偷的挑逗著我，傳達著非常明顯的訊號"; break;
            case 3 : lustText = "動作扭扭捏捏的，和平常比起來更容易害羞，即使我現在馬上對她下手她也會乖乖配合吧"; break;
            case 4 : lustText = "此時對於任何事務都很敏感，對話中不乏渴求著什麼的語氣，肢體語言表露出著肉眼可見的慾火焚身"; break;
        }

        conditionText = friendshipText + lustText;
        return conditionText;
    }

    private int LustDetected(int lustNumber)
    {
        if (numericalData.lust == 0)
        {
            lustNumber = 0;
        }
        else if(numericalData.lust >= 1 && numericalData.lust <= 10)
        {
            lustNumber = 1;
        }
        else if(numericalData.lust >= 11 && numericalData.lust <= 20)
        {
            lustNumber = 2;
        }
        else if(numericalData.lust >= 21 && numericalData.lust <= 35)
        {
            lustNumber = 3;
        }
        else if(numericalData.lust >= 36)
        {
            lustNumber = 4;
        }

        return lustNumber;
    }
}

public class GameTextBoxDiaData
{
    public int DailogIndex;
    public string ActorName;
    public string Dailog;
}
