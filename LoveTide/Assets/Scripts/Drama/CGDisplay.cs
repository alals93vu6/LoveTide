using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CGDisplay : MonoBehaviour
{
    [SerializeField] public Image CGimg;
    [SerializeField] public GameObject backGroundImg;
    [SerializeField] private int CGOrder;
    [SerializeField] private int backGroundOrder;
    [SerializeField] private DialogData dialog;
    [SerializeField] private int targetDialog;
    [SerializeField] private float CGalpha;
    [SerializeField] private bool isDisplayCG;
    
    // Start is called before the first frame update
    async void Start()
    {
        await Task.Delay(100);
        DisplayBackGroundChick(0);
    }

    private void FixedUpdate()
    {
        if (isDisplayCG)
        {
            CGalpha += 1.4f * Time.deltaTime;
        }
        else
        {
            CGalpha -= 1.4f * Time.deltaTime;
        }
        CGimg.color = new Color(CGimg.color.r, CGimg.color.g, CGimg.color.b, CGalpha);
        CGalpha = Mathf.Clamp(CGalpha, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnStart(DialogData diadata , int targetNumber)
    {
        dialog = diadata;
        
        CGOrder = 0;
        targetDialog = targetNumber;
        DisplayCGChick(dialog.plotOptionsList[targetDialog].dialogDataDetails[0].switchCGDisplay,
            dialog.plotOptionsList[targetDialog].dialogDataDetails[0].switchCGImage);
    }

    public async void DisplayCGChick(bool ShowCG,bool SwitchCG)
    {
        //Debug.Log(ShowCG);
        //Debug.Log(SwitchCG);
        if (ShowCG)
        {
            if (!isDisplayCG)
            {
                var textBoxCtrl = FindObjectOfType<PlayerCtrlDrama>();
                isDisplayCG = true;
                textBoxCtrl.DisplayTextBox(1);
                await Task.Delay(1000);
                textBoxCtrl.DisplayTextBox(2);
            }
            else
            {
                isDisplayCG = false;
            }
        }
        if (SwitchCG && CGOrder < dialog.plotOptionsList[targetDialog].displayCG.Length -1)
        {
            CGOrder++;
        }
        
        if (dialog.plotOptionsList[targetDialog].displayCG.Length > 0)
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
