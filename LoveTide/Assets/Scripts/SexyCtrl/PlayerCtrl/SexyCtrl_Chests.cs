using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SexyCtrl_Chests : MonoBehaviour
{
    [SerializeField] public Text testText;
    [SerializeField] private IState CurrenState = new IdleState_Chests();
    [SerializeField] public string[] stateAnimator;
    [SerializeField] public float nowSpeed;
    public float oldSpeed;
    private bool isDetected;
    [SerializeField] public bool haveMouth;
    // Start is called before the first frame update
    void Start()
    {
        testText = GetComponent<Text>();
        CurrenState.OnEnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        CurrenState.OnStayState(this);
        if (oldSpeed != nowSpeed)
        {
            isDetected = true;
            if (isDetected)
            {
                SwitchAnimator();
                oldSpeed = nowSpeed;
                isDetected = false;
            }
        }
    }
    
    public void SwitchAnimator()
    {
        switch (nowSpeed)
        {
            case 0: testText.text = stateAnimator[0] ; break;
            case 1: testText.text = stateAnimator[1] ; break;
            case 2: testText.text = stateAnimator[2] ; break;
            case 3: testText.text = stateAnimator[3] ; break;
        }
    }
    
    public void ChangeState(IState nextState)
    {
        CurrenState.OnExitState(this);
        nextState.OnEnterState(this);
        CurrenState = nextState;
    }
}
