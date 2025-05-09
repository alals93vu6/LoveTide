﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class bgmManager : MonoBehaviour
{
    #region Instance
    static public bgmManager instance;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    [SerializeField] public AudioSource[] backgroundMusic;
    [SerializeField] public AudioSource audioTrackA;
    [SerializeField] public AudioSource audioTrackB;
    [SerializeField] public float testFloatA;
    [SerializeField] public float testFloatB;
    [SerializeField] public bool isAudioA;
    
    // Start is called before the first frame update
    [ContextMenu("MusicTest")]
    private void Test()
    {
        if (isAudioA)
        {
            SwitchAudio(2);
        }
        else
        {
            SwitchAudio(1);
        }
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (isAudioA)
        {
            audioTrackA.volume = Mathf.Lerp(audioTrackA.volume, PlayerPrefs.GetInt("bgmSet") /10f, 0.08f);
            audioTrackB.volume = Mathf.Lerp(audioTrackB.volume, 0, 0.08f);
        }
        else
        {
            audioTrackB.volume = Mathf.Lerp(audioTrackB.volume, PlayerPrefs.GetInt("bgmSet") /10f, 0.08f);
            audioTrackA.volume = Mathf.Lerp(audioTrackA.volume, 0, 0.08f);
        }

        if (isAudioA)
        {
            if (audioTrackB.volume <= 0.05f)
            {
                audioTrackB.volume = 0f;
            }
        }
        else
        {
            if (audioTrackA.volume <= 0.05f)
            {
                audioTrackA.volume = 0f;
            }
        }
    }

    public void PlayAudio(int audioNumber)
    {
        backgroundMusic[audioNumber].Play();
    }

    public void SwitchAudio(int targetAudioNumber)
    {
        if (isAudioA)
        {
            isAudioA = false;
            audioTrackB = backgroundMusic[targetAudioNumber];
            audioTrackB.Play();
        }
        else
        {
            isAudioA = true;
            audioTrackA = backgroundMusic[targetAudioNumber];
            audioTrackA.Play();
        }
    }

    public void SetAudioVolume()
    {
        if (isAudioA)
        {
            audioTrackA.volume = PlayerPrefs.GetInt("bgmSet") /10f ;
        }
        else
        {
            audioTrackB.volume = PlayerPrefs.GetInt("bgmSet") /10f ;
        }
    }
}
