using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CheckCGManager : MonoBehaviour
{
    [SerializeField] public int nowPage;
    [SerializeField] public int cgPage;
    [SerializeField] public int maxPage;
    [SerializeField] public bool isDisplay;
    [SerializeField] private ClickCGObject cgButton;
    [SerializeField] private PageObjectCtrl pageCtrl;
    [SerializeField] public DirtyTrickCtrl darkCtrl;
    [SerializeField] private GameObject uiObject;
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

    public async void StartWatch(int instantiateNumber)
    {
        cgPage = 1;
        isDisplay = true;
        darkCtrl.OnChangeScenes();
        GetComponent<InstantiateManager>().OnInstantiateObject(instantiateNumber);
        await Task.Delay(500);
        //watchCG.gameObject.SetActive(true);
        uiObject.SetActive(false);
        SwitchWatch(0);
        //Debug.Log(instantiateNumber);
    }

    public void SetMaxPage(int setMax)
    {
        maxPage = setMax;
    }


    public void SwitchWatch(int setPage)
    {
        var nowPage = cgPage += setPage;
        if (nowPage <= 0)
        {
            cgPage = 1;
        }
        else  if(nowPage > maxPage)
        {
            QuitWatch();
        }
        else
        {
            cgPage = nowPage;
            FindObjectOfType<DisplayObjectCtrl>().OnSwitchImage(cgPage);
        }
    }

    private async void QuitWatch()
    {
        isDisplay = false;
        darkCtrl.OnChangeScenes();
        await Task.Delay(500);
        uiObject.SetActive(true);
        FindObjectOfType<DisplayObjectCtrl>().OnQuitWatchImage();
        //watchCG.gameObject.SetActive(false);
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
