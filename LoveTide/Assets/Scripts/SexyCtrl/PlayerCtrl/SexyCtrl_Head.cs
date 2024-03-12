using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SexyCtrl_Head : MonoBehaviour
{
    [SerializeField] public Text testText;
    [SerializeField] private IState CurrenState = new IdleState_Face();
    [SerializeField] public bool onKiss;
    [SerializeField] public float nowSpeed;
    // Start is called before the first frame update
    void Start()
    {
        testText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (onKiss)
        {
            testText.text = "G表情:接吻";
        }
        else
        {
            testText.text = "G表情:待機";
        }

        CurrenState.OnStayState(this);
    }
    
    public void ChangeState(IState nextState)
    {
        CurrenState.OnExitState(this);
        nextState.OnEnterState(this);
        CurrenState = nextState;
    }
}
