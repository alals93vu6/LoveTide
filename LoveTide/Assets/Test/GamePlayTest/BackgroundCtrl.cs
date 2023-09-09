using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundCtrl : MonoBehaviour
{
    [SerializeField] public Image displayImage;
    [SerializeField] public GameObject[] sceneObject;
    [SerializeField] private Sprite[] backgrounds;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChickBackground(int detectedTimePoint)
    {
        
        var gameMNG = FindObjectOfType<GameManagerTest>();
        if (gameMNG.isTalk)
        {
            switch (detectedTimePoint)
            {
                case 1: SwitchBackground(1); break;
                case 2: SwitchBackground(2); break;
                case 3: SwitchBackground(3); break;
                case 4: SwitchBackground(4); break;
                case 5: SwitchBackground(5); break;
                case 6: SwitchBackground(6); break;
                case 7: SwitchBackground(7); break;
                case 8: SwitchBackground(8); break;
                case 9: SwitchBackground(3); break;
            }
        }
        else
        {
            switch (detectedTimePoint)
            {
                case 1: SwitchBackground(1); break;
                case 2: SwitchBackground(2); break;
                case 3: SwitchBackground(3); break;
                case 4: SwitchBackground(4); break;
                case 5: SwitchBackground(5); break;
                case 6: SwitchBackground(6); break;
                case 7: SwitchBackground(7); break;
                case 8: SwitchBackground(8); break;
                case 9: SwitchBackground(3); break;
            }
        }

        
    }
    
    

    public void SwitchBackground(int backgroundNumber)
    {
        displayImage.GetComponent<Image>().sprite = backgrounds[backgroundNumber];
    }
}
