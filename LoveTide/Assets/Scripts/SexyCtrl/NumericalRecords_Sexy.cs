using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NumericalRecords_Sexy : MonoBehaviour
{
    [Header("基底數值")]
    [SerializeField] public float playerDelight;//快感
    [SerializeField] public bool isMotionPlayer;
    [SerializeField] public float girlDelight;//快感
    [SerializeField] public int orgasmNumber;
    [SerializeField] public bool isMotionGirl;

    [Header("浮動數值")] 
    [SerializeField] public float playerStamina;//體力
    [SerializeField] public float girlStamina;//體力

    [Header("物件")] 
    [SerializeField] private PlayerActor_Sexy playerActor;
    [SerializeField] private SexyUIManager sexyUICtrl;
    [SerializeField] public NumericalRecords numericalManager;
    // Start is called before the first frame update
    private async void Awake()
    {
        
    }

    void Start() 
    {
        playerStamina = GetStamina(true, 0);
        girlStamina = GetStamina(false, 0);
        InvokeRepeating("DelightDetected_Girl",0f,1f);
        InvokeRepeating("DelightDetected_Player",0f,1f);
    }

    // Update is called once per frame
    void Update()
    {
        sexyUICtrl.PlayerStatusUIDisplay(playerStamina / GetStamina(true,0),playerDelight / 100);
        sexyUICtrl.GirlStatusUIDisplay(girlStamina / GetStamina(false,0),girlDelight / 100);
    }
    
    public void DelightDetected_Player()
    {
        if (playerActor.motionSpeed != 0)
        {
            playerDelight += 1 * SpeedDetected_PlayerMotion(0);
        }
        else
        {
            if (playerDelight > 0.5f)
            {
                playerDelight -= 0.5f;
            }
        }
    }
    
    public void DelightDetected_Girl()
    {
        if (isMotionGirl)
        {
            girlDelight += (0.25f * SpeedDetected_GirlMotion(0)) * GetGirlSensitivity(0);
        }
        else
        {
            if (playerDelight > 0.5f)
            {
                playerDelight -= 0.5f;
            }
        }
    }

    private float SpeedDetected_PlayerMotion(float getNumber)
    {
        switch (playerActor.motionSpeed)
        {
            case 0 : getNumber = 0f; break;
            case 1 : getNumber = 0.55f; break;
            case 2 : getNumber = 0.8f; break;
            case 3 : getNumber = 1.5f; break;
        }

        return getNumber;
    }

    private float SpeedDetected_GirlMotion(float getNumber)
    {
        switch (playerActor.motionSpeed)
        {
            case 0 : getNumber = 0f; break;
            case 1 : getNumber = 0.55f; break;
            case 2 : getNumber = 0.8f; break;
            case 3 : getNumber = 1.5f; break;
        }
        
        return getNumber;
    }

    private float GetGirlSensitivity(float getNumber)
    {
        getNumber = ((1f + (numericalManager.slutty * 0.003f)) + (numericalManager.lust * 0.01f)) * (1 + orgasmNumber * 0.1f) ;

        return getNumber;
    }

    private float GetStamina(bool isPlayer,float getNumber)
    {
        if (isPlayer)
        {
            getNumber = 2 + (numericalManager.friendship * 0.0015f);
        }
        else
        {
            getNumber = 1 + (numericalManager.friendship * 0.0038f);
        }
        
        return getNumber;
    }
}
