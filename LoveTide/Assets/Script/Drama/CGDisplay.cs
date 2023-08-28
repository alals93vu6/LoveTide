using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGDisplay : MonoBehaviour
{
    [SerializeField] public GameObject CGimg;
    [SerializeField] private int CGOrder;
    [SerializeField] private DialogData dialog;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStart(DialogData diadata)
    {
        dialog = diadata;
        CGimg.SetActive(false);
        CGOrder = 0;
        DisplayCGChick(dialog.plotOptionsList[0].dialogDataDetails[0].switchCGDisplay,dialog.plotOptionsList[0].dialogDataDetails[0].switchCGImage);
    }

    public void DisplayCGChick(bool ShowCG,bool SwitchCG)
    {
        Debug.Log(ShowCG);
        Debug.Log(SwitchCG);
        
        if (ShowCG)
        {
            if (CGimg.activeSelf)
            {
                CGimg.SetActive(false);
            }
            else
            {
                CGimg.SetActive(true);
            }
        }
        if (SwitchCG && CGOrder < dialog.plotOptionsList[0].displayCG.Length -1)
        {
            CGOrder++;
        }
        
        if (CGimg.activeSelf)
        {
            CGimg.GetComponent<Image>().sprite = dialog.plotOptionsList[0].displayCG[CGOrder];
        }
        
    }
}
