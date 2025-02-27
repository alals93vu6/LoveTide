﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLocation : MonoBehaviour
{
    [SerializeField] private string locationNumber;
    [SerializeField] private int intNumber;
    [Header("檔案數值")] 
    [SerializeField] public Text aDay;
    [SerializeField] public Text friendship;//好感度
    [SerializeField] public Text slutty;//淫亂度
    [SerializeField] public Text lust;//慾望值
    [SerializeField] public Text playTime;//慾望值
    [SerializeField] public GameObject[] clickButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayDataInformation(bool isContinue)
    {
        aDay.text = "目前天數 : " + PlayerPrefs.GetInt("aDayData" + locationNumber);
        friendship.text = "好感度 : " +  PlayerPrefs.GetInt("friendshipData" + locationNumber);
        slutty.text = "淫亂值 : " +  PlayerPrefs.GetInt("sluttyData" + locationNumber);
        lust.text = "慾望值 : " +  PlayerPrefs.GetInt("lustData" + locationNumber);
        PlayTimeConversion();
        DisplayChoseButton(isContinue);
    }

    public void DisplayChoseButton(bool isContinue)
    {
        for (int i = 0; i < clickButton.Length; i++)
        {
            clickButton[i].SetActive(false);
        }
        if (isContinue)
        {
            if (PlayerPrefs.GetInt("aDayData" + locationNumber) == 0)
            {
                clickButton[3].SetActive(true);
                clickButton[4].SetActive(true);
            }
            else
            {
                clickButton[1].SetActive(true);
                clickButton[2].SetActive(true);
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("aDayData" + locationNumber) == 0)
            {
                clickButton[0].SetActive(true);
            }
            else
            {
                clickButton[3].SetActive(true);
                clickButton[4].SetActive(true);
            }
        }
    }

    private void PlayTimeConversion()
    {
        var allTime = (int)PlayerPrefs.GetFloat("PlayTimeData" + locationNumber);
        //var allTime = (int)PlayerPrefs.GetFloat("PlayTimeData0");
        var getSecond = allTime % 60;
        var getMinutes = (allTime % 3600) / 60 ;
        var getHour = allTime / 3600;
        playTime.text = "遊玩時長 : " +  getHour + "h : " + getMinutes + "m : " + getSecond + "s";
    }

    public async void LoadThisData()
    {
        FindObjectOfType<DirtyTrickCtrl>().OnExitGamePlayScenes();
        PlayerPrefs.SetInt("GameDataNumber",intNumber);
        await Task.Delay(3000);
        if (PlayerPrefs.GetInt("mainMissionEvent" + locationNumber) == 0)
        {
            PlayerPrefs.SetInt("DramaNumber", 1);
            PlayerPrefs.SetString("playerNameData" + locationNumber,"阿金");
            SceneManager.LoadScene("DramaScene");
        }
        else
        {
            SceneManager.LoadScene("Scenes/GamePlayScene");
        }
    }

    public void CoverThisData()
    {
        string dataNumber = PlayerPrefs.GetInt("GameDataNumber").ToString();
        PlayerPrefs.SetInt("aDayData" + locationNumber,PlayerPrefs.GetInt("aDayData" + dataNumber));
        PlayerPrefs.SetInt("aTimerData" + locationNumber,PlayerPrefs.GetInt("aTimerData" + dataNumber));
        PlayerPrefs.SetInt("aWeekData" + locationNumber,PlayerPrefs.GetInt("aWeekData" + dataNumber));
        PlayerPrefs.SetInt("friendshipData" + locationNumber,PlayerPrefs.GetInt("friendshipData" + dataNumber));
        PlayerPrefs.SetInt("sluttyData" + locationNumber,PlayerPrefs.GetInt("sluttyData" + dataNumber));
        PlayerPrefs.SetInt("lustData" + locationNumber,PlayerPrefs.GetInt("lustData" + dataNumber));
        PlayerPrefs.SetInt("partyEvent" + locationNumber,PlayerPrefs.GetInt("partyEvent" + dataNumber));
        PlayerPrefs.SetInt("outingEvent" + locationNumber,PlayerPrefs.GetInt("outingEvent" + dataNumber));
        PlayerPrefs.SetInt("mainMissionEvent" + locationNumber,PlayerPrefs.GetInt("mainMissionEvent" + dataNumber));
        PlayerPrefs.SetInt("tavernData" + locationNumber,PlayerPrefs.GetInt("tavernData" + dataNumber));
        PlayerPrefs.SetInt("dormitoriesData" + locationNumber,PlayerPrefs.GetInt("dormitoriesData" + dataNumber));
        PlayerPrefs.SetInt("beachData" + locationNumber,PlayerPrefs.GetInt("beachData" + dataNumber));
        PlayerPrefs.SetInt("hillsData" + locationNumber,PlayerPrefs.GetInt("hillsData" + dataNumber));
        PlayerPrefs.SetInt("shoppingStreetData" + locationNumber,PlayerPrefs.GetInt("shoppingStreetData" + dataNumber));
        PlayerPrefs.SetInt("parkData" + locationNumber,PlayerPrefs.GetInt("parkData" + dataNumber));
        PlayerPrefs.SetInt("PropsLevelData" + locationNumber,PlayerPrefs.GetInt("PropsLevelData" + dataNumber));
        PlayerPrefs.SetFloat("PlayTimeData" + locationNumber,PlayerPrefs.GetFloat("PlayTimeData" + dataNumber));
        PlayerPrefs.SetString("playerNameData" + locationNumber,PlayerPrefs.GetString("playerNameData" + dataNumber));
        PlayerPrefs.SetInt("GameDataNumber",intNumber);
    }
}


    /*  string dataNumber = PlayerPrefs.GetInt("GameDataNumber").ToString();
        PlayerPrefs.SetInt("aDayData" + dataNumber,PlayerPrefs.GetInt("aDayData" + locationNumber));
        PlayerPrefs.SetInt("aTimerData" + dataNumber,PlayerPrefs.GetInt("aTimerData" + locationNumber));
        PlayerPrefs.SetInt("aWeekData" + dataNumber,PlayerPrefs.GetInt("aWeekData" + locationNumber));
        PlayerPrefs.SetInt("friendshipData" + dataNumber,PlayerPrefs.GetInt("friendshipData" + locationNumber));
        PlayerPrefs.SetInt("sluttyData" + dataNumber,PlayerPrefs.GetInt("sluttyData" + locationNumber));
        PlayerPrefs.SetInt("lustData" + dataNumber,PlayerPrefs.GetInt("lustData" + locationNumber));
        PlayerPrefs.SetInt("partyEvent" + dataNumber,PlayerPrefs.GetInt("partyEvent" + locationNumber));
        PlayerPrefs.SetInt("outingEvent" + dataNumber,PlayerPrefs.GetInt("outingEvent" + locationNumber));
        PlayerPrefs.SetInt("mainMissionEvent" + dataNumber,PlayerPrefs.GetInt("mainMissionEvent" + locationNumber));
        PlayerPrefs.SetInt("tavernData" + dataNumber,PlayerPrefs.GetInt("tavernData" + locationNumber));
        PlayerPrefs.SetInt("dormitoriesData" + dataNumber,PlayerPrefs.GetInt("dormitoriesData" + locationNumber));
        PlayerPrefs.SetInt("beachData" + dataNumber,PlayerPrefs.GetInt("beachData" + locationNumber));
        PlayerPrefs.SetInt("hillsData" + dataNumber,PlayerPrefs.GetInt("hillsData" + locationNumber));
        PlayerPrefs.SetInt("shoppingStreetData" + dataNumber,PlayerPrefs.GetInt("shoppingStreetData" + locationNumber));
        PlayerPrefs.SetInt("parkData" + dataNumber,PlayerPrefs.GetInt("parkData" + locationNumber));
        PlayerPrefs.SetInt("PropsLevelData" + dataNumber,PlayerPrefs.GetInt("PropsLevelData" + locationNumber));
        PlayerPrefs.SetFloat("PlayTimeData" + dataNumber,PlayerPrefs.GetFloat("PlayTimeData" + locationNumber));
        PlayerPrefs.SetString("playerNameData" + dataNumber,PlayerPrefs.GetString("playerNameData" + locationNumber));*/