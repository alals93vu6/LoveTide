using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickObj : MonoBehaviour
{
    [SerializeField] private int eventNumber;
    [SerializeField] private int setFDS;
    [SerializeField] private int setSLT;
    [SerializeField] private int setLST;
    [SerializeField] private int FDSlv;
    // Start is called before the first frame update

    private void Start()
    {
        FDSlv = PlayerPrefs.GetInt("FDS_LV");
        switch (eventNumber)
        {
            case 12 : TalkNumberCtrl(); break;
            case 2 : OperateNumberCtrl(); break;
            case 3 : OlirtTalkNumberCtrl(); break;
            case 4 : MolestNumberCtrl(); break;
            case 5 : PeekingNumberCtrl(); break;
        }
    }

    private void OnMouseDown()
    {
        FindObjectOfType<PlayerActorTest>().OnClickActor(eventNumber,setFDS,setSLT,setLST);
    }

    private void TalkNumberCtrl()
    {
        switch (FDSlv)
        {
            case 0:
                setFDS += 2;
                setSLT += 0;
                setLST += 0;
                break;
            case 1:
                setFDS += 3;
                setSLT += 0;
                setLST += 0;
                break;
            case 2:
                setFDS += 5;
                setSLT += 0;
                setLST += 0;
                break;
            case 3:
                setFDS += 5;
                setSLT += 0;
                setLST += 0;
                break;
            case 4:
                setFDS += 7;
                setSLT += 0;
                setLST += 0;
                break;
        }
    }
    
    private void OperateNumberCtrl()
    {
        switch (FDSlv)
        {
            case 0:
                setFDS += 2;
                setSLT += 0;
                setLST += 0;
                break;
            case 1:
                setFDS += 3;
                setSLT += 0;
                setLST += 0;
                break;
            case 2:
                setFDS += 5;
                setSLT += 0;
                setLST += 0;
                break;
            case 3:
                setFDS += 5;
                setSLT += 0;
                setLST += 0;
                break;
            case 4:
                setFDS += 7;
                setSLT += 0;
                setLST += 0;
                break;
        }
    }
    
    private void OlirtTalkNumberCtrl()
    {
        switch (FDSlv)
        {
            case 0:
                setFDS += 3;
                setSLT += 0;
                setLST += 0;
                break;
            case 1:
                setFDS += 5;
                setSLT += 0;
                setLST += 0;
                break;
            case 2:
                setFDS += 5;
                setSLT += 0;
                setLST += 1;
                break;
            case 3:
                setFDS += 7;
                setSLT += 0;
                setLST += 2;
                break;
            case 4:
                setFDS += 8;
                setSLT += 0;
                setLST += 2;
                break;
        }
    }
    
    private void MolestNumberCtrl()
    {
        switch (FDSlv)
        {
            case 0:
                setFDS += -5;
                setSLT += 0;
                setLST += 0;
                break;
            case 1:
                setFDS += -2;
                setSLT += 0;
                setLST += 0;
                break;
            case 2:
                setFDS += 0;
                setSLT += 0;
                setLST += 1;
                break;
            case 3:
                setFDS += 1;
                setSLT += 0;
                setLST += 3;
                break;
            case 4:
                setFDS += 3;
                setSLT += 0;
                setLST += 5;
                break;
        }
    }
    
    private void PeekingNumberCtrl()
    {
        switch (FDSlv)
        {
            case 0:
                setFDS += 0;
                setSLT += 0;
                setLST += 0;
                break;
            case 1:
                setFDS += 0;
                setSLT += 0;
                setLST += 0;
                break;
            case 2:
                setFDS += 0;
                setSLT += 0;
                setLST += 0;
                break;
            case 3:
                setFDS += 0;
                setSLT += 0;
                setLST += 0;
                break;
            case 4:
                setFDS += 0;
                setSLT += 0;
                setLST += 0;
                break;
        }
    }


}
