using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundCtrl : MonoBehaviour
{
    [SerializeField] public Image displayImage;
    [SerializeField] public GameObject[] sceneObject;
    [SerializeField] private Sprite[] backgrounds;
    [SerializeField] private Sprite[] outingBackgrounds;

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
            case 9: SwitchBackground(9); break;
            case 10: SwitchBackground(10); break;
        }
    }
    
    public void ChickBackground_Outing(int detectedTimePoint)
    {
        switch (detectedTimePoint)
        {
            case 0: SwitchBackground_Outing(0); break;
            case 1: SwitchBackground_Outing(1); break;
            case 2: SwitchBackground_Outing(2); break;
            case 3: SwitchBackground_Outing(3); break;
            case 4: SwitchBackground_Outing(4); break;
            case 5: SwitchBackground_Outing(5); break;
            case 6: SwitchBackground_Outing(6); break;
            case 7: SwitchBackground_Outing(7); break;
            case 8: SwitchBackground_Outing(8); break;
            case 9: SwitchBackground_Outing(9); break;
            case 10: SwitchBackground_Outing(10); break;
            case 11: SwitchBackground_Outing(11); break;
            case 12: SwitchBackground_Outing(12); break;
            case 13: SwitchBackground_Outing(13); break;
        }
    }
    
    

    public void SwitchBackground(int backgroundNumber)
    {
        displayImage.GetComponent<Image>().sprite = backgrounds[backgroundNumber];
    }
    
    public void SwitchBackground_Outing(int backgroundNumber)
    {
        displayImage.GetComponent<Image>().sprite = outingBackgrounds[backgroundNumber];
    }
}
