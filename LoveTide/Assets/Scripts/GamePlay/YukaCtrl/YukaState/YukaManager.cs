using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class YukaManager : MonoBehaviour
{
    [SerializeField] public bool isInCounter;
    [SerializeField] public bool readyGoToCounter;
    public IState CurrenState = new YukaIdleState();
    
    [SerializeField] public SkeletonAnimation yukaAnimator;
    [SerializeField] public string[] idleAnimator = { "YukaQ_Idle2", "YukaQ_Idle_smile" };   /*{ "YukaQ_Idle2", "YukaQ_Idle_smile" };*/
    [SerializeField] public string[] walkAnimator = { "YukaQ_Walk", "YukaQ_Walk_smile" };
    
    [SerializeField] public GameObject[] wayPoint;
    [SerializeField] public int wayTarget;
    [SerializeField] public float wayDistance;
    [SerializeField] private float previousX;
    [SerializeField] private float switchTime;

    /*[SerializeField] private float numberR;
    [SerializeField] private float numberG;
    [SerializeField] private float numberB;*/

    void Start()
    {
        OnStart();
    }

    void Update()
    {
        CounterDetected();
        WayDistanceDetected();
    }

    private void FixedUpdate()
    {
        CurrenState.OnStayState(this);
        SwitchStayLocation();
        FlipDetected();
    }

    private void SwitchStayLocation()
    {
        switchTime += Time.deltaTime;
        if (switchTime >= 40)
        {
            if (readyGoToCounter)
            {
                readyGoToCounter = false;
            }
            else
            {
                readyGoToCounter = true;
            }
            switchTime = 0f;
        }
    }

    private void FlipDetected()
    {
        float currentX = transform.position.x;
        
        if (currentX > previousX)
        {
            yukaAnimator.gameObject.transform.rotation = Quaternion.Euler(0,180,0);
        }
        else
        {
            yukaAnimator.gameObject.transform.rotation = Quaternion.Euler(0,0,0);
        }
        previousX = currentX;
    }

    private void CounterDetected()
    {
        if (gameObject.transform.position.y >= 210)
        {
            isInCounter = true;
        }
        else
        {
            isInCounter = false;
        }
    }

    private void OnStart()
    {
        var startPosition = Random.Range(1, 12);
        var counterDetected = Random.Range(1, 3);
        if (counterDetected == 1)
        {
            readyGoToCounter = true;
        }
        else
        {
            readyGoToCounter = false;
        }
        transform.position = wayPoint[startPosition].gameObject.transform.position;
        wayTarget = startPosition;
        SetColor();
    }

    private void SetColor()
    {
        yukaAnimator.skeleton.SetColor(new Color(0.85f,0.85f,0.75f,1));
    }

    public void SwitchPosition()
    {
        int nowTarget;
        if (readyGoToCounter)
        {
            if (isInCounter)
            {
                nowTarget = Random.Range(1,4);
            }
            else
            {
                if (transform.position.x <= 650)
                {
                    nowTarget = 5;
                }
                else
                {
                    nowTarget = 4;
                }
            }
        }
        else
        {
            if (isInCounter)
            {
                if (transform.position.x <= 650)
                {
                    nowTarget = 4;
                }
                else
                {
                    nowTarget = 5;
                }
            }
            else
            {
                nowTarget = Random.Range(6,11);
            }
        }

        if (nowTarget == wayTarget)
        {
            SwitchPosition();
        }
        else
        {
            wayTarget = nowTarget;
        }
    }

    private void WayDistanceDetected()
    {
        wayDistance = Vector3.Distance(transform.position, wayPoint[wayTarget].gameObject.transform.position);
    }

    public void ChangeState(IState nextState)
    {
        CurrenState.OnExitState(this);
        nextState.OnEnterState(this);
        CurrenState = nextState;
    }
}
