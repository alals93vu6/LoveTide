using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameUICtrlmanager : MonoBehaviour
{
    [SerializeField] public DirtyTrickCtrl darkCtrl;
    [SerializeField] public InformationUI_ClickObject informationButton;
    [SerializeField] public GameManagerTest gameManager;
    [SerializeField] public NumericalRecords_PlayerStting playerSetNumerical;

    [SerializeField] public GameObject informationButtonObject;
    [SerializeField] public GameObject settingsSystemObject;
    [SerializeField] public GameObject[] settingsSlider;
    // Start is called before the first frame update
    async void Start()
    {
        await Task.Delay(1000);
        DisplaySettings(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplaySettings(bool isOpen)
    {
        if (isOpen)
        {
            gameManager.SetClickObject(0);
            settingsSystemObject.SetActive(false);
        }
        else
        {
            FirstSetCheck();
            settingsSystemObject.SetActive(true);
        }
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
        gameManager.SetClickObject(7);
        for (int i = 0; i < settingsSlider.Length; i++)
        {
            settingsSlider[i].GetComponent<SliderValuDisplay>().OnOpen();
        }
    }
}
