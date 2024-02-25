using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SexyCtrl_Head : MonoBehaviour
{
    [SerializeField] private Text testText;
    // Start is called before the first frame update
    void Start()
    {
        testText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
