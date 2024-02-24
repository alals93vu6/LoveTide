using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioChoseSystem : MonoBehaviour
{
    #region Instance
    static public ScenarioChoseSystem instance;
    private void Awake()
    {
        instance = this;
        dataFile = "LogA";
    }
    #endregion
    
    [SerializeField] public string dataFile;
    // Start is called before the first frame update
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
