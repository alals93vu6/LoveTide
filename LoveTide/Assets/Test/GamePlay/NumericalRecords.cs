using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumericalRecords : MonoBehaviour
{
    [Header("玩家數值")]
    [SerializeField] public int aDay;
    [SerializeField] public int aTimer;
    [SerializeField] public int aWeek;
    [SerializeField] public int friendship;//好感度
    [SerializeField] public int slutty;//淫亂度
    [SerializeField] public int lust;//慾望值

    [Header("事件經歷")] 
    [SerializeField] public int party;//趴踢
    [SerializeField] public int alonOuting;//出去玩
    [SerializeField] public int mainMission;//主線

    [ContextMenu("AAA")]
    private void ASASA()
    {
        FindObjectOfType<GameManagerTest>().SetClickObject(0);
    }

    public void OnStart()
    {
        GameDataLoad(PlayerPrefs.GetInt("GameDataNumber").ToString());
        FDS_Detected();
        
    }
    
    public void GameDataLoad(string dataNumber)
    {
        aDay = PlayerPrefs.GetInt("aDayData" + dataNumber);
        aTimer = PlayerPrefs.GetInt("aTimerData" + dataNumber);
        aWeek = PlayerPrefs.GetInt("aWeekData" + dataNumber);
        friendship = PlayerPrefs.GetInt("friendshipData" + dataNumber);
        slutty = PlayerPrefs.GetInt("sluttyData" + dataNumber);
        lust = PlayerPrefs.GetInt("lustData" + dataNumber);
        party = PlayerPrefs.GetInt("partyEvent" + dataNumber);
        alonOuting = PlayerPrefs.GetInt("outingEvent" + dataNumber);
        mainMission = PlayerPrefs.GetInt("mainMissionEvent" + dataNumber);
        
        if (aWeek == 0)
        {
            aWeek = 1;
        }
        
        if (aTimer == 0)
        {
            aTimer = 1;
        }
    }

    public void FDS_Detected()
    {
        if (friendship <= 200)
        {
            PlayerPrefs.SetInt("FDS_LV",0);//290  
        }
        else if(friendship >201 && friendship <= 500)//+150 =150
        {
            PlayerPrefs.SetInt("FDS_LV",1);//560
        }
        else if(friendship >501 && friendship <= 750)//+150 +200 = 350
        {
            PlayerPrefs.SetInt("FDS_LV",2);//900 = 560 + 140 + 200
        }
        else if(friendship >751 && friendship <= 1900)//350 +200 = 550
        {
            PlayerPrefs.SetInt("FDS_LV",3);//X = 900 + 950 +  ~ 315(210 ~ 420)
        }
        else if(friendship >1901)//  (550 +200 +200 +200) 1150 +350  = 1500
        {
            PlayerPrefs.SetInt("FDS_LV",4);
        }
    }

    public void GameDataSave()
    {
        string dataNumber = PlayerPrefs.GetInt("GameDataNumber").ToString();
        PlayerPrefs.SetInt("aDayData" + dataNumber,aDay);
        PlayerPrefs.SetInt("aTimerData" + dataNumber,aTimer);
        PlayerPrefs.SetInt("aWeekData" + dataNumber,aWeek);
        PlayerPrefs.SetInt("friendshipData" + dataNumber,friendship);
        PlayerPrefs.SetInt("sluttyData" + dataNumber,slutty);
        PlayerPrefs.SetInt("lustData" + dataNumber,lust);
        PlayerPrefs.SetInt("partyEvent" + dataNumber,party);
        PlayerPrefs.SetInt("outingEvent" + dataNumber,alonOuting);
        PlayerPrefs.SetInt("mainMissionEvent" + dataNumber,mainMission);
    }
    
    
    public void GameDataReset()
    {
        string dataNumber = PlayerPrefs.GetInt("GameDataNumber").ToString();
        PlayerPrefs.SetInt("aDayData" + dataNumber,1);
        PlayerPrefs.SetInt("aTimerData" + dataNumber,1);
        PlayerPrefs.SetInt("aWeekData" + dataNumber,1);
        PlayerPrefs.SetInt("friendshipData" + dataNumber,0);
        PlayerPrefs.SetInt("sluttyData" + dataNumber,0);
        PlayerPrefs.SetInt("lustData" + dataNumber,0);
        PlayerPrefs.SetInt("partyEvent" + dataNumber,0);
        PlayerPrefs.SetInt("outingEvent" + dataNumber,0);
        PlayerPrefs.SetInt("mainMissionEvent" + dataNumber,0);
    }


    // Start is called before the first frame update
    public void SetNumerica(int fds,int slt, int lst)
    {
        friendship += fds;
        slutty += slt;
        lust += lst;
    }

}
