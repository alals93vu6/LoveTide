using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUPUIManager : MonoBehaviour
{
    [SerializeField] private GameObject configure;
    [SerializeField] public GameObject[] settingsSlider;
    [SerializeField] public Dropdown[] setDropdowns;
    // Start is called before the first frame update
    private void Start()
    {
        FirstSetCheck();
        bgmManager.instance.SwitchAudio(1);
    }

    public void ClickConfigure(bool isOpen)
    {
        configure.SetActive(isOpen);
    }
    
    private void FirstSetCheck()
    {
        if (PlayerPrefs.GetInt("firstSetting") == 0)
        {
            PlayerPrefs.SetInt("bgmSet",5);
            PlayerPrefs.SetInt("soundSet",5);
            PlayerPrefs.SetInt("voicesSet",5);
            PlayerPrefs.SetInt("firstSetting",1);
            OnOpenSetPage();
        }
        else
        {
            OnOpenSetPage();
        }
    }

    private void OnOpenSetPage()
    {
        for (int i = 0; i < settingsSlider.Length; i++)
        {
            settingsSlider[i].GetComponent<SliderValuDisplay>().OnOpen();
        }
        setDropdowns[0].GetComponent<SwitchGameWindows>().OnOpen();
        setDropdowns[1].GetComponent<LanguageSet>().OnOpen();
    }
}
