using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishingmanager : MonoBehaviour
{
    [Header("釣魚")] 
    [SerializeField] private FishObject[] fish;
    
    
    [Header("狀態")] 
    [SerializeField] private IState CurrenState = new Fishing_IdleState();
    [SerializeField] public float loseTime;
    [SerializeField] public bool QTEon;
    [SerializeField] public bool isStop;
    
    [Header("魚種資訊")]
    [SerializeField] public float maxStamina;
    [SerializeField] public float nowStamina;
    [SerializeField] public float damageReduction;
    
    [Header("功能")]
    [SerializeField] public QTESliderManager sliderQTE;
    [SerializeField] public StaminaComponent fishStamina;
    [SerializeField] public LevelSetingComponent levelSet;
    [SerializeField] public Animator fishActor;
    
    // Start is called before the first frame update
    void Start()
    {
        nowStamina = maxStamina;
    }

    private void Update()
    {
        CurrenState.OnStayState(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isStop && QTEon)
        {
            sliderQTE.QTESliderComponent();
            if (sliderQTE.QTECtrl.isinArea)
            {
                loseTime = 0f;
                nowStamina -= Time.deltaTime * damageReduction;
            }
            else
            {
                loseTime += Time.deltaTime;
            }
        }
        
        nowStamina = Mathf.Max(0, nowStamina);
        fishStamina.DisplayFishStaminaComponent(maxStamina,nowStamina);
    }

    public void ChangeState(IState nextState)
    {
        CurrenState.OnExitState(this);
        nextState.OnEnterState(this);
        CurrenState = nextState;
    }
}
