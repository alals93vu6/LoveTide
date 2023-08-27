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
        DisplayCGChick(dialog.dialogDataDetails[0].SwitchCGDisplay,dialog.dialogDataDetails[0].SwitchCGImage);
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
        if (SwitchCG && CGOrder < dialog.disPlayCG.Length -1)
        {
            CGOrder++;
        }
        
        if (CGimg.activeSelf)
        {
            CGimg.GetComponent<Image>().sprite = dialog.disPlayCG[CGOrder];
        }
        
    }
}
