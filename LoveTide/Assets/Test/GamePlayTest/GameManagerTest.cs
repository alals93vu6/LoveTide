using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerTest : MonoBehaviour
{
    [SerializeField] private NumericalRecords numberCtrl;
    [SerializeField] private BackgroundCtrl background;
    [SerializeField] private TimeManagerTest timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DayPassedEvent(int fds,int slt, int lst)
    {
        numberCtrl.SetNumerica(fds,slt,lst);
        timer.DetectedDayPassed();
    }

    public void TimeOffWork()
    {
        
    }
}
