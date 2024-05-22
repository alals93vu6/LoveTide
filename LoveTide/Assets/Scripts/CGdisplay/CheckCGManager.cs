using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCGManager : MonoBehaviour
{
    [SerializeField] public int nowPage;
    [SerializeField] private ClickCGObject cgButton;
    [SerializeField] private PageObjectCtrl pageCtrl;
    // Start is called before the first frame update
    void Start()
    {
        ChangePage(true);
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
