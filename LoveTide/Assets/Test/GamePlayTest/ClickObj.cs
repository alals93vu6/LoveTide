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
    // Start is called before the first frame update
    private void OnMouseDown()
    {
        FindObjectOfType<PlayerActorTest>().OnClickActor(eventNumber,setFDS,setSLT,setLST);
    }
    
    
    
    
}
