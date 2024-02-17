using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor : MonoBehaviour
{

    [Header("狀態")] 
    [SerializeField] private bool A;
    [SerializeField] private IState CurrenState = new IdleState();
    [SerializeField] private int actionState;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CurrenState.OnStayState(this);
    }
    
    public void ChangeState(IState nextState)
    {
        //Debug.Log("StateChange");
        CurrenState.OnExitState(this);
        nextState.OnEnterState(this);
        CurrenState = nextState;
    }
}
