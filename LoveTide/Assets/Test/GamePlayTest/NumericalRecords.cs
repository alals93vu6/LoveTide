using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumericalRecords : MonoBehaviour
{
    [SerializeField] public int aDay;
    [SerializeField] public int aTimer;
    [SerializeField] public int aWeek;
    [SerializeField] public int friendship;
    [SerializeField] public int slutty;
    [SerializeField] public int lust;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNumerica(int fds,int slt, int lst)
    {
        friendship += fds;
        slutty += slt;
        lust += lst;
    }

}
