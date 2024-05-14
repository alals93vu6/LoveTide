using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchGameWindows : MonoBehaviour
{
    [SerializeField] public Dropdown detectedDropdown;
    [SerializeField] private bool fullWindows;
    // Start is called before the first frame update
    void Start()
    {
        detectedDropdown.onValueChanged.AddListener(OnSwitchWindows);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnOpen()
    {
        detectedDropdown.value = PlayerPrefs.GetInt("FullWindows");
    }

    public void OnSwitchWindows(int optionIndex)
    {
        switch (optionIndex)
        {
            case 0:
                Screen.SetResolution(1920,1080,true);
                PlayerPrefs.SetInt("FullWindows",0);
                break;
            case 1:
                //Screen.fullScreen = false;
                Screen.SetResolution(1280,720,false);
                PlayerPrefs.SetInt("FullWindows",1);
                break;
        }
    }
}
