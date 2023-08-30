using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManagerTest : MonoBehaviour
{
    [SerializeField] private NumericalRecords numberCtrl;
    [SerializeField] public bool vacation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void DetectedDayPassed()
    {
        if (numberCtrl.aWeek >= 7){numberCtrl.aWeek = 1;}else{numberCtrl.aWeek++;}
        numberCtrl.aTimer = 1;
        numberCtrl.aDay++;
    }

    public void VacationDetected()
    {
        if (numberCtrl.aWeek < 3 || numberCtrl.aWeek > 4)
        {
            vacation = true;
        }
        else
        {
            DetectedPeriod();
        }
    }

    public void DetectedPeriod()
    {
        if (numberCtrl.aTimer >= 7)
        {
            vacation = true;
        }
        else
        {
            vacation = false;
        }
    }
}
