using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetDataTest : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Text displayText;
    [SerializeField] public int TestNumber;
    [SerializeField] private string TestName;
    [SerializeField] private int asNumber;
    
    void Start()
    {
        PlayerPrefs.SetInt("TestObjA",1);
        PlayerPrefs.SetInt("TestObjB",3);
        PlayerPrefs.SetInt("TestObjC",5);
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (TestNumber)
        {
            case 1: TestName = "A"; break;
            case 2: TestName = "B"; break;
            case 3: TestName = "C"; break;
        }
        
        asNumber = PlayerPrefs.GetInt("TestObj" + TestName);

        displayText.text = asNumber.ToString();
    }
}
