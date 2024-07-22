using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishingmanager : MonoBehaviour
{
    [Header("釣魚")] 
    [SerializeField] private FishObject[] fish;
    
    
    [Header("狀態")] 
    [SerializeField] public bool QTEon;
    [SerializeField] public bool isStop;
    
    [Header("魚種資訊")] 
    [SerializeField] private float maxStamina;
    [SerializeField] private float nowStamina;
    [SerializeField] private float damageReduction;
    
    [Header("功能")]
    [SerializeField] private QTESliderManager sliderQTE;
    [SerializeField] private StaminaComponent fishStamina;
    
    // Start is called before the first frame update
    void Start()
    {
        nowStamina = maxStamina;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            sliderQTE.OnQTEDetected(QTEon);
            if (QTEon)
            {
                QTEon = false;
            }
            else
            {
                QTEon = true;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.E) && QTEon)
        {
            if (isStop)
            {
                isStop = false;
            }
            else
            {
                isStop = true;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isStop && QTEon)
        {
            sliderQTE.QTESliderComponent();
            if (sliderQTE.QTECtrl.isinArea)
            {
                nowStamina -= Time.deltaTime * damageReduction;
            }
        }
        
        nowStamina = Mathf.Max(0, nowStamina);
        fishStamina.DisplayFishStaminaComponent(maxStamina,nowStamina);
    }

    
}
