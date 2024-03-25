using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager_Sexy : MonoBehaviour
{
    [SerializeField] private PlayerActor_Sexy playerCtrl;
    [SerializeField] private NumericalRecords_Sexy numericalCtrl;
    // Start is called before the first frame update
    private void Awake()
    {
        numericalCtrl.numericalManager.OnStart();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
