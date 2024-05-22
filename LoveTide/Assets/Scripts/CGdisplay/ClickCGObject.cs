using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickCGObject : MonoBehaviour
{
    [SerializeField] private GameObject[] clickButtonPage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchPage(int nowPage)
    {
        for (int i = 0; i < clickButtonPage.Length; i++)
        {
            clickButtonPage[i].SetActive(false);
        }
        
        clickButtonPage[nowPage].SetActive(true);
    }
}
