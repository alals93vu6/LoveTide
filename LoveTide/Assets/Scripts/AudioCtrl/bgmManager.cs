using System;
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
        SwitchAudio(1);
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAudio(int audioNumber)
    {
        backgroundMusic[audioNumber].Play();
    }

    public async void SwitchAudio(int targetAudioNumber)
    {
        if (isAudioA)
        {
            isAudioA = false;
            audioTrackB = backgroundMusic[targetAudioNumber];
            while (testFloatA <= 0.5f)
            {
                testFloatA += Time.deltaTime;
            }

            await Task.Delay(1000);
            
            while (testFloatB >= 0.5f)
            {
                testFloatB += Time.deltaTime;
            }
        }
        else
        {
            isAudioA = true;
            audioTrackA = backgroundMusic[targetAudioNumber];
            while (testFloatB <= 0.5f)
            {
                testFloatB += Time.deltaTime;
            }

            await Task.Delay(1000);
            
            while (testFloatA >= 0.5f)
            {
                testFloatA += Time.deltaTime;
            }
        }
    }
}
