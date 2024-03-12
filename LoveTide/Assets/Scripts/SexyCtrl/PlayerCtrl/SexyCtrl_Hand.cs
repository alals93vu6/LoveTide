using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SexyCtrl_Hand : MonoBehaviour
{
    [SerializeField] public Text testText;
    [SerializeField] private IState CurrenState = new IdleState_Hand();
    [SerializeField] public float nowSpeed;
    // Start is called before the first frame update
    void Start()
    {
        testText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        CurrenState.OnStayState(this);
    }
    
    public void ChangeState(IState nextState)
    {
        CurrenState.OnExitState(this);
        nextState.OnEnterState(this);
        CurrenState = nextState;
    }
}
