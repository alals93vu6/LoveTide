using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SexyButton : MonoBehaviour
{
    [SerializeField] private Text buttonText;
    // Start is called before the first frame update

    public void TextDetected()
    {
        if (PlayerPrefs.GetInt("FDS_LV") < 2)
        {
            if (FindObjectOfType<NumericalRecords>().notRape == 0)
            {
                if (FindObjectOfType<TimeManagerTest>().vacation)
                {
                    buttonText.text = "捉i";
                }
                else
                {
                    buttonText.text = "-";
                }
            }
            else
            {
                buttonText.text = "-";
            }
        }
    }
}
