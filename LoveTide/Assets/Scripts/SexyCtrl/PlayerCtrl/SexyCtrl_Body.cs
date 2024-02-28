using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SexyCtrl_Body : MonoBehaviour
{
    [SerializeField] public Text testText;
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
