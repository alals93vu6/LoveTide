using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PageObjectCtrl : MonoBehaviour
{
    [SerializeField] private GameObject[] pageShowObject;
    // Start is called before the first frame update
    void Start()
    {
        SwitchPage(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchPage(int nowPage)
    {
        for (int i = 0; i < pageShowObject.Length; i++)
        {
            pageShowObject[i].GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
        }
        pageShowObject[nowPage].GetComponent<Image>().color = new Vector4(0.72f, 0.5f, 0.32f, 1);
    }
}
