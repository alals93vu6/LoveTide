using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownTest : MonoBehaviour
{
    [SerializeField] public Dropdown testDrop;
    // Start is called before the first frame update
    void Start()
    {
        testDrop.onValueChanged.AddListener(TestEvent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TestEvent(int optionIndex)
    {
        switch (optionIndex)
        {
            case 0:
                Debug.Log("AAA");
                break;
            case 1:
                Debug.Log("BBB");
                break;
        }
    }
}
