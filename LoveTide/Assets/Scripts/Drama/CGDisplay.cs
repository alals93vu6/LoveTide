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
    [SerializeField] private int targetDialog;
    
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
    
    public void OnStart(DialogData diadata , int targetNumber)
    {
        dialog = diadata;
        CGimg.SetActive(false);
        CGOrder = 0;
        targetDialog = targetNumber;
        DisplayCGChick(dialog.plotOptionsList[targetDialog].dialogDataDetails[0].switchCGDisplay,
            dialog.plotOptionsList[targetDialog].dialogDataDetails[0].switchCGImage);
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
        if (SwitchCG && CGOrder < dialog.plotOptionsList[targetDialog].displayCG.Length -1)
        {
            CGOrder++;
        }
        
        if (CGimg.activeSelf)
        {
            CGimg.GetComponent<Image>().sprite = dialog.plotOptionsList[targetDialog].displayCG[CGOrder];
        }
        //Debug.Log(dialog.plotOptionsList[targetDialog].displayCG[CGOrder].name);
        
    }
    
    public void DisplayBackGroundChick(int setOrder)
    {
        backGroundOrder += setOrder;
        backGroundImg.GetComponent<Image>().sprite = dialog.plotOptionsList[targetDialog].displayBackground[backGroundOrder];
    }
}
