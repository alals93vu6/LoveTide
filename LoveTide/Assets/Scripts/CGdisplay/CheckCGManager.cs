using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckCGManager : MonoBehaviour
{
    [SerializeField] public int nowPage;
    [SerializeField] public int cgPage;
    [SerializeField] public bool isDisplay;
    [SerializeField] public WatchImage watchCG;
    [SerializeField] private ClickCGObject cgButton;
    [SerializeField] private PageObjectCtrl pageCtrl;
    // Start is called before the first frame update
    void Start()
    {
        ChangePage(true);
    }

    private void Update()
    {
        if (isDisplay)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                SwitchWatch(1);
            }
        
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SwitchWatch(-1);
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                QuitWatch();
            }
        }
    }

    public void StartWatch()
    {
        cgPage = 1;
        isDisplay = true;
        watchCG.gameObject.SetActive(true);
        SwitchWatch(0);
    }

    public void SwitchWatch(int setPage)
    {
        var nowPage = cgPage += setPage;
        if (nowPage <= 0)
        {
            cgPage = 1;
            watchCG.GetComponent<Image>().sprite = watchCG.cgImage.ActorStandingDrawing[cgPage];
        }
        else  if(nowPage >= watchCG.cgImage.ActorStandingDrawing.Length)
        {
            QuitWatch();
        }
        else
        {
            cgPage = nowPage;
            watchCG.GetComponent<Image>().sprite = watchCG.cgImage.ActorStandingDrawing[cgPage];
        }
    }

    public void QuitWatch()
    {
        isDisplay = false;
        watchCG.gameObject.SetActive(false);
    }
    
    public void ChangePage(bool isAdd)
    {
        if (isAdd)
        {
            if (nowPage + 1  < 6)
            {
                nowPage++;
                pageCtrl.SwitchPage(nowPage);
                cgButton.SwitchPage(nowPage);
            }
        }
        else
        {
            if (nowPage - 1 > 0)
            {
                nowPage--;
                pageCtrl.SwitchPage(nowPage);
                cgButton.SwitchPage(nowPage);
            }
        }
    }
    
    

}
