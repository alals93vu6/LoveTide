using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SliderQTE_TargetArea : MonoBehaviour
{
    [SerializeField] private float moveHorizontal;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float onTime;
    [SerializeField] private float duration;

    public void TargetAreaComponent()
    {
        AreaMove();
        ChangeTimeDetected();
    }

    private void AreaMove()
    {
        Vector3 newPosition = transform.position + Vector3.right * moveHorizontal * moveSpeed * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, -2.5f, 2.5f);
        transform.position = newPosition;
    }

    public void ChangeMoveMode()
    {
        onTime = 0f;
        var theWay = 0;
        if (transform.position.x >= -1.5f && transform.position.x <= 1.5f)
        {
            theWay = Random.Range(1,3);
            duration = Random.Range(0.8f, 1.2f);
        }
        else if (transform.position.x <= -1.5f)
        {
            theWay = 1;
            duration = Random.Range(1f, 1.5f);
        }
        else if (transform.position.x >= 1.5f)
        {
            theWay = 2;
            duration = Random.Range(0.8f, 1.6f);
        }

        if (theWay == 1)
        {
            moveHorizontal = 1;
        }
        else
        {
            moveHorizontal = -1;
        }
        moveSpeed = Random.Range(1.5f, 2.6f);
    }

    private void ChangeTimeDetected()
    {
        onTime += Time.deltaTime;
        if (onTime >= duration)
        {
            ChangeMoveMode();
        }
    }
    
    public void ResetPosition()
    {
        transform.localPosition = new Vector3(0,0,0);
        moveSpeed = 0f;
        onTime = 0f;
    }
}
