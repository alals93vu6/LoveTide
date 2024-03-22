using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sexyctrl_GirlHand : MonoBehaviour
{
    [SerializeField] public Text testText;
    [SerializeField] public IState CurrenState = new IdleState_GirlHand();
    [SerializeField] public string[] stateAnimator;
    [SerializeField] public float nowSpeed;
    [SerializeField] public Button[] limitationButtons;
    public float oldSpeed;
    private bool isDetected;
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

    public void OnLimitation(bool isLimitation)
    {
        for (int i = 0; i < limitationButtons.Length; i++)
        {
            limitationButtons[i].interactable = isLimitation;
        }
    }

    public void SwitchStateDetected(int detectedNumber)
    {
        if (CurrenState is IdleState_GirlHand)
        {
            if (detectedNumber == 1)
            {
                ChangeState(new OnGraspState_GirlHand());
            }
            else if (detectedNumber == 2)
            {
                ChangeState(new OnHoldHandState_GirlHand());
            }
        }
        else if (CurrenState is OnGraspState_GirlHand)
        {
            if (detectedNumber == 1)
            {
                ChangeState(new IdleState_GirlHand());
            }
            else if (detectedNumber == 2)
            {
                ChangeState(new OnHoldHandState_GirlHand());
            }
        }
        else if (CurrenState is OnHoldHandState_GirlHand)
        {
            if (detectedNumber == 1)
            {
                ChangeState(new OnGraspState_GirlHand());
            }
            else if (detectedNumber == 2)
            {
                ChangeState(new IdleState_GirlHand());
            }
        }
    }

    public void ChangeState(IState nextState)
    {
        CurrenState.OnExitState(this);
        nextState.OnEnterState(this);
        CurrenState = nextState;
    }
}
