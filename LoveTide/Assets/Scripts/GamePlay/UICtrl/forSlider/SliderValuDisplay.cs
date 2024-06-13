using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValuDisplay : MonoBehaviour
{
    [SerializeField] public Text displayText;
    [SerializeField] private int sliderNumber;
    [SerializeField] public NumericalRecords_PlayerStting recordsPlayerSetting;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DisplaySetNumber();
        if (Input.GetMouseButtonUp(0))
        {
            ChangeSetDetected();
        }
    }

    private void DisplaySetNumber()
    {
        displayText.text = "" + this.GetComponent<Slider>().value;
    }

    private void ChangeSetDetected()
    {
        float saveNumber = this.GetComponent<Slider>().value;
        switch (sliderNumber)
        {
            case 1: PlayerPrefs.SetInt("bgmSet",(int)saveNumber); break;
            case 2: PlayerPrefs.SetInt("soundSet",(int)saveNumber); break;
            case 3: PlayerPrefs.SetInt("voicesSet",(int)saveNumber); break;
        }

        if (sliderNumber == 1)
        {
            bgmManager.instance.SetAudioVolume();
        }
    }

    public void OnOpen()
    {
        this.GetComponent<Slider>().value = FindValue(sliderNumber);
    }

    private int FindValue(int getNumber)
    {
        int returnNumber = 0;
        recordsPlayerSetting.LoadGameSetData();
        switch (getNumber)
        {
            case 1: returnNumber= recordsPlayerSetting.bgmLoudness; break;
            case 2: returnNumber= recordsPlayerSetting.soundLoudness; break;
            case 3: returnNumber= recordsPlayerSetting.voicesLoudness; break;
        }
        //Debug.Log(returnNumber);
        return returnNumber;
    }
}
