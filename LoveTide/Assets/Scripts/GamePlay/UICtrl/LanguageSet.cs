using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSet : MonoBehaviour
{
    [SerializeField] public Dropdown languageSet;
    // Start is called before the first frame update
    void Start()
    {
        languageSet.onValueChanged.AddListener(OnSwitchLanguage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnOpen()
    {
        languageSet.value = PlayerPrefs.GetInt("LanguageSet");
    }

    public void OnSwitchLanguage(int optionIndex)
    {
        switch (optionIndex)
        {
            case 0:
                PlayerPrefs.SetInt("LanguageSet",0);
                break;
            case 1:
                PlayerPrefs.SetInt("LanguageSet",1);
                break;
            case 2:
                PlayerPrefs.SetInt("LanguageSet",2);
                break;
            case 3:
                PlayerPrefs.SetInt("LanguageSet",3);
                break;
        }

        FindObjectOfType<DialogDataManager>().OnLoadDialogData(FindObjectOfType<NumericalRecords>().DialogueDetected());
    }
}
