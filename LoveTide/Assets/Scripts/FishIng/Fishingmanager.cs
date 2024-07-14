using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishingmanager : MonoBehaviour
{
    [Header("釣魚")] 
    [SerializeField] private FishObject[] fish;

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

    // Update is called once per frame
    void FixedUpdate()
    {
        nowStamina -= Time.deltaTime * damageReduction;
        nowStamina = Mathf.Max(0, nowStamina);
        fishStamina.DisplayFishStaminaComponent(maxStamina,nowStamina);
    }

    
}
