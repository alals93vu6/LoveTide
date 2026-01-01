using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] public GameUICtrlmanager gameUICtrl;
    [SerializeField] public DialogDataManager dialogManager;

    [Header("狀態")] 
    [SerializeField] public bool isTalk;
    [SerializeField] public bool timePass;
    [SerializeField] public bool isAlone;
    [SerializeField] private bool getEvent;
    [SerializeField] public bool getSexy;
    [SerializeField] public int talkIndex;
    
    


    private void Awake()
    {
        //Debug.Log(PlayerPrefs.GetString("playerNameData" + PlayerPrefs.GetInt("GameDataNumber").ToString()));
        numberCtrl.OnStart();
        dialogManager.OnLoadDialogData(numberCtrl.DialogueDetected());
        CheckActions();
    }

    // Start is called before the first frame update
     void Start()
     {
         OnStart();
         //PlayerPrefs.DeleteAll();
     }

    private void Update()
    {
       /* if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
        {
            numberCtrl.GameDataSave();
            Debug.Log("SaveData");
        }
        
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            numberCtrl.GameDataReset();
            Debug.Log("ResetData");
            SceneManager.LoadScene(0);
        }*/
    }
    
    public async void OnStart()
    {
        if (FindObjectOfType<bgmManager>() != null)
        {
            bgmManager.instance.SwitchAudio(2);
        }
        timer.VacationDetected();
        await Task.Delay(50);
        SetClickObject(0);
        CheckActions();
        numberCtrl.isPlayGame = true;  
    }

    public async void TimePassCheck()
    {
        if (numberCtrl.aTimer >= 10)
        {
            if (timer.vacation)
            {
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
                DayPassedEvent(0,0,0);
            }
        }
        else
        {
            if (numberCtrl.aTimer == 7 && !timer.vacation)
            {
                GameEventDetected();
                if (getEvent)
                {
                    gameUICtrl.darkCtrl.OnChangeScenes();
                    numberCtrl.GameDataSave();
                    await Task.Delay(500);
                    SceneManager.LoadScene("DramaScene");
                }
                else
                {
                    isTalk = false;
                    SetInteractiveObject(false);
                    OnTalkEvent(61);
                    timer.vacation = true;
                }
            }
            else if (numberCtrl.aTimer == 8 && !isAlone)
            {
                isTalk = false;
                gameUICtrl.darkCtrl.OnChangeScenes();
                await Task.Delay(500);
                SetInteractiveObject(false);
                OnTalkEvent(68);
                isAlone = true;
            }
            else
            {
                SetClickObject(0);
                CheckActions();
            }
        }
    }

    public async void DayPassedEvent(int fds,int slt, int lst)
    {
        timer.DetectedDayPassed();
        GameEventDetected();
        if (getEvent)
        {
            numberCtrl.SetNumerica(fds,slt,lst);
            gameUICtrl.darkCtrl.OnChangeScenes();
            numberCtrl.GameDataSave();
            await Task.Delay(500);
            SceneManager.LoadScene("DramaScene");
        }
        else
        {
            numberCtrl.SetNumerica(fds,slt,lst);
            gameUICtrl.darkCtrl.OnChangeScenes();
            await Task.Delay(500);
            SetClickObject(0);
            CheckActions();
            numberCtrl.GameDataSave();
            switch (numberCtrl.aWeek)
            {
                case 1: OnTalkEvent(58); break;
                case 2: OnTalkEvent(59); break;
                case 3: OnTalkEvent(60); break;
                case 4: OnTalkEvent(60); break;
                case 5: OnTalkEvent(55); break;
                case 6: OnTalkEvent(56); break;
                case 7: OnTalkEvent(57); break;
            }
        }
    }

    public void ClickTextBoxEvent()
    {
        if (textBox.isover)
        {
            textBox.DownText();
        }
        else
        {
            talkIndex++;
            textBox.NextText();
            if (textBox.textNumber < textBox.gameTextBoxDiaDatas.Count - 1)
            {
                actorManager.ActorCtrl(timer.vacation, GetAction());
            }
        }
    }

    public async void HaveSexyEvent()
    {
        FindObjectOfType<DirtyTrickCtrl>().OnExitGamePlayScenes();
        await Task.Delay(750);
        SceneManager.LoadScene("Scenes/SexyCtrl");
    }

    public void OnTalkEvent(int talkID)
    {
        talkIndex = 1;
        textBox.listSerial = talkID;
        textBox.OnDisplayText(dialogManager.DiaDataList);
        SetClickObject(6);
        gameUICtrl.informationButtonObject.SetActive(false);
        if (numberCtrl.aTimer == 7 && !timer.vacation) { }
        else if (numberCtrl.aTimer == 10 && timer.vacation) { }
        else
        {
            CheckActions();
        } 
        actorCtrl.StayTarget = 0;
        
        if (talkID >= 59 && talkID != 61)
        {
            SetInteractiveObject(false);
        }
        else
        {
            actorCtrl.gameObject.SetActive(true);
            interactiveButton[0].SetActive(false);
            actorManager.ActorCtrl(timer.vacation, GetAction());
        }
    }
    
    public async void TalkDownEvent()
    {
        if (textBox.listSerial == 61 || textBox.listSerial == 67)
        {
            FindObjectOfType<DirtyTrickCtrl>().OnChangeScenes();
            await Task.Delay(500);
        }

        if (timePass)
        {
            numberCtrl.aTimer++;
            timePass = false;
        }
        
        if (getSexy)
        {
            HaveSexyEvent();
        }
        else
        {
            textBox.stopLoop = false;
            actorCtrl.StayTarget = 1;
            actorManager.ActorCtrl(timer.vacation, "Normal");
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
    }

    private string GetAction()
    {
        // 先抓出符合 EventIndex 與 DailogIndex 的那筆資料
        var matchData = dialogManager.DiaDataList
            .FirstOrDefault(data => data.EventIndex == textBox.listSerial
                                 && data.DialogIndex == talkIndex);

        // 若沒有符合資料則直接回傳預設 "NoActor"
        if (matchData == null)
            return "NoActor";

        // 特殊條件：若角色是 Player 或 Narrator，且 DialogIndex == 1 且 ActorFace == "NoActor"
        if ((matchData.ActorName == "Player" || matchData.ActorName == "Narrator")
            && matchData.DialogIndex == 1
            && matchData.ActorFace == "NoActor")
        {
            //Debug.Log($"特殊事件為：{textBox.listSerial}  編號為：{talkIndex}  表情為：{matchData.ActorFace}");
            return "Normal";
        }

        // 其他情況就照原本邏輯回傳 ActorFace
        //Debug.Log($"事件為：{textBox.listSerial}  編號為：{talkIndex}  表情為：{matchData.ActorFace}");
        return matchData.ActorFace ?? "NoActor";

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
        if (isActive)
        {
            FindObjectOfType<SexyButton>().TextDetected();
        }
    }

    public void SetClickObject(int displayObject)
    {
        for (int i = 0; i < sceneObject.Length; i++)
        {
            sceneObject[i].SetActive(false);
        }
        //gameUICtrl.informationButtonObject.SetActive(false);

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
    private void GameEventDetected()
    {
        switch (numberCtrl.aDay)
        {
            case 3 :
                PlayerPrefs.SetInt("DramaNumber",1);
                getEvent = true;
                break;
            case 6 :
                PlayerPrefs.SetInt("DramaNumber",1);
                getEvent = true;
                break;
            case 15 :
                if (numberCtrl.aTimer >= 6)
                {
                    PlayerPrefs.SetInt("DramaNumber",1);
                    getEvent = true;
                }
                break;
            case 24 :
                if (numberCtrl.aTimer >= 6)
                {
                    PlayerPrefs.SetInt("DramaNumber",1);
                    getEvent = true;
                }
                break;
            case 32 :
                if (numberCtrl.aTimer <= 3)
                {
                    PlayerPrefs.SetInt("DramaNumber",1);
                    getEvent = true;
                    
                }
                break;
            case 37 :
                PlayerPrefs.SetInt("DramaNumber",1);
                getEvent = true;
                break;
            case 45 :
                PlayerPrefs.SetInt("DramaNumber",1);
                getEvent = true;
                break;
            case 47 :
                if (numberCtrl.aTimer >= 6)
                {
                    PlayerPrefs.SetInt("DramaNumber",1);
                    getEvent = true;
                }
                break;
            case 49 :
                if (numberCtrl.aTimer >= 6)
                {
                    PlayerPrefs.SetInt("DramaNumber",1);
                    getEvent = true;
                }
                break;
            case 51 :
                PlayerPrefs.SetInt("DramaNumber",1);
                getEvent = true;
                break;
        }
    }

    public void CheckupsButton()
    {
        OnTalkEvent(67);
    }
    
    public void SaveGameDataButton()
    {
        numberCtrl.GameDataSave();
        OnTalkEvent(66);
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
        gameUICtrl.informationButtonObject.SetActive(false);
        
        if (objectNumber == 1 || objectNumber == 3)
        {
            gameUICtrl.informationButtonObject.SetActive(true);
        }
        
        //Debug.Log(objectNumber);
        return objectNumber;
    }

}
