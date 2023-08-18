using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTimeTest : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void logTime()
    {
        var NowTimeData = DateTime.Now;
        Debug.Log(NowTimeData.Year+"/"+NowTimeData.Month+"/"+NowTimeData.Day+"   "+NowTimeData.Minute+"："+NowTimeData.Second);
    }
}
