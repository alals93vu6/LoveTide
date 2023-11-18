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
    }

    public void GameDataLoad(string dataNumber)
    {
        aDay = PlayerPrefs.GetInt("aDayData" + dataNumber);
        aTimer = PlayerPrefs.GetInt("aTimerData" + dataNumber);
        aWeek = PlayerPrefs.GetInt("aWeekData" + dataNumber);
        friendship = PlayerPrefs.GetInt("friendshipData" + dataNumber);
        slutty = PlayerPrefs.GetInt("sluttyData" + dataNumber);
        lust = PlayerPrefs.GetInt("lustData" + dataNumber);

        if (aWeek == 0)
        {
            aWeek = 1;
        }
        
        if (aTimer == 0)
        {
            aTimer = 1;
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
    }


    // Start is called before the first frame update
    public void SetNumerica(int fds,int slt, int lst)
    {
        friendship += fds;
        slutty += slt;
        lust += lst;
    }

}
