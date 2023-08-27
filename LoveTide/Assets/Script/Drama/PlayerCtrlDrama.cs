using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrlDrama : MonoBehaviour
{
    [Header("物件A")]
    [SerializeField] private TextBoxDrama texBox;
    [SerializeField] private ActorManagerDrama actorCtrl;
    [SerializeField] private CGDisplay CGDisplay;
    
    [Header("物件B")]
    [SerializeField] public DialogData diaLog;
    [SerializeField] private ScenarioChoseSystem scenarioChose;

    [SerializeField] private GameObject texBoxObj;

    [Header("數值")] [SerializeField] private int talkOrder;
    // Start is called before the first frame update
    void Start()
    {
        diaLog = Resources.Load<DialogData>(scenarioChose.dataFile);
        texBox.OnStart_TextBox(diaLog);
        actorCtrl.OnStart(diaLog);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerClick();
    }

    private void PlayerClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetTextBox(true);
            if (texBox.isover)
            {
                texBox.DownText();
            }
            else
            {
                texBox.NextText();
                actorCtrl.ActorCtrl(diaLog.dialogDataDetails[talkOrder].stayLocation);
                talkOrder = texBox.textNumber;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (texBoxObj.activeSelf)
            {
                SetTextBox(false);
            }
            else
            {
                SetTextBox(true);
            }
        }
    }

    private void SetTextBox(bool isDisplay)
    {
        texBoxObj.SetActive(isDisplay);
    }
    
    
}
