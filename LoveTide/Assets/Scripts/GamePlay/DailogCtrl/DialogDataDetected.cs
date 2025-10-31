using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogDataDetected : MonoBehaviour
{
    public string DiaLogListDetected(int progress)
    {
        return $"PlayingDialog/{LanguageDetected()}/{ProgressDetected(progress)}";
    }

    private string LanguageDetected()
    {
        string lang = "EN";
        switch (PlayerPrefs.GetInt("LanguageSet"))
        {
            case 0: lang = "EN"; break;
            case 1: lang = "JP"; break;
            case 2: lang = "TW"; break;
            case 3: lang = "CN"; break;
        }
        return lang;
    }

    private string ProgressDetected(int progress)
    {
        string prog = "Pro1";
        switch (progress)
        {
            case 0: prog = "GrowMode_1"; break;
            case 1: prog = "GrowMode_2"; break;
            case 2: prog = "GrowMode_3"; break;
            case 3: prog = "GrowMode_4"; break;
            case 4: prog = "GrowMode_5"; break;
            case 5: prog = "GrowMode_6"; break;
        }
        return prog;
    }


}
