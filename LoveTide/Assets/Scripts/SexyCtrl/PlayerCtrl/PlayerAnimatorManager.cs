using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviour
{
    [SerializeField] public SexyCtrl_Head headCtrl;
    [SerializeField] public SexyCtrl_Body bodyCtrl;
    [SerializeField] public SexyCtrl_Chests leftChestsCtrl;
    [SerializeField] public SexyCtrl_Chests rightChestsCtrl;
    [SerializeField] public SexyCtrl_Hand leftHandCtrl;
    [SerializeField] public SexyCtrl_Hand rightHandCtrl;
    [SerializeField] public Sexyctrl_GirlHand girlHandCtrl;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    

    public void SetAnimatorMotionSpeed(float motionSpeed)
    {
        headCtrl.nowSpeed = bodyCtrl.nowSpeed = leftChestsCtrl.nowSpeed = rightChestsCtrl.nowSpeed 
            = leftChestsCtrl.nowSpeed = rightHandCtrl.nowSpeed = leftHandCtrl.nowSpeed = girlHandCtrl.nowSpeed = motionSpeed;
    }
}
