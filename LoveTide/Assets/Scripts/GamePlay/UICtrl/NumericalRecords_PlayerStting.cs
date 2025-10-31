using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumericalRecords_PlayerStting : MonoBehaviour
{
    [Header("設定保存")] 
    [SerializeField] public int bgmLoudness;
    [SerializeField] public int soundLoudness;
    [SerializeField] public int voicesLoudness;
    [SerializeField] public int languageLoudness;

    // Start is called before the first frame update
    private void Awake()
    {
        LoadGameSetData();
    }

    public void LoadGameSetData()
    {
        bgmLoudness = PlayerPrefs.GetInt("bgmSet");
        soundLoudness = PlayerPrefs.GetInt("soundSet");
        voicesLoudness = PlayerPrefs.GetInt("voicesSet");
        languageLoudness = PlayerPrefs.GetInt("LanguageSet");
    }
}
