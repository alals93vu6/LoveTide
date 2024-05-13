using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class GameUICtrlmanager : MonoBehaviour
{
    [SerializeField] public DirtyTrickCtrl darkCtrl;
    [SerializeField] public InformationUI_ClickObject informationButton;
    [SerializeField] public GameManagerTest gameManager;

    [SerializeField] public GameObject informationButtonObject;
    [SerializeField] public GameObject settingsSystemObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DisplaySettings(false);
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
            gameManager.SetClickObject(7);
            settingsSystemObject.SetActive(true);
        }
        
    }
}
