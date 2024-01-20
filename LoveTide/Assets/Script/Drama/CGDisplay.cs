using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CGDisplay : MonoBehaviour
{
    [SerializeField] public GameObject CGimg;
    [SerializeField] public GameObject backGroundImg;
    [SerializeField] private int CGOrder;
    [SerializeField] private int backGroundOrder;
    [SerializeField] private DialogData dialog;
    
    // Start is called before the first frame update
    async void Start()
    {
        await Task.Delay(100);
        DisplayBackGroundChick(0);
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
        //Debug.Log(ShowCG);
        //Debug.Log(SwitchCG);
        
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
    
    public void DisplayBackGroundChick(int setOrder)
    {
        backGroundOrder += setOrder;
        backGroundImg.GetComponent<Image>().sprite = dialog.plotOptionsList[0].displayBackground[backGroundOrder];
    }
}
