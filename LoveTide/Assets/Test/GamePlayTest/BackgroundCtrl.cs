using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundCtrl : MonoBehaviour
{
    [SerializeField] public Image displayImage;
    [SerializeField] private Sprite[] backgrounds;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchBackground(int backgroundNumber)
    {
        displayImage.GetComponent<Image>().sprite = backgrounds[backgroundNumber];
    }
}
