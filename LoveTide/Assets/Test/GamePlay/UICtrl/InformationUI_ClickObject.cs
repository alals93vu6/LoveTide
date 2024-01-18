using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationUI_ClickObject : MonoBehaviour
{
    [SerializeField] public bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOpen)
        {
            transform.localPosition = Vector3.Lerp(this.transform.localPosition,new Vector3(-550,195,0),0.05f);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(this.transform.localPosition,new Vector3(-260,195,0),0.05f);
        }
    }

    public void OnSwitchButton()
    {
        if (isOpen)
        {
            isOpen = false;
        }
        else
        {
            isOpen = true;
        }
    }
}
/* this.transform.position = Vector3.Lerp(transform.position,
                new Vector3(200, 0, 0), 0.02f);*/