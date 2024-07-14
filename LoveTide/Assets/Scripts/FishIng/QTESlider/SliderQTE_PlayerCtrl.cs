using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderQTE_PlayerCtrl : MonoBehaviour
{
    public bool isinArea;
    // Update is called once per frame
    private void Update()
    {
        
    }

    public void PlayerMove()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 newPosition = transform.position + Vector3.right * horizontalInput * 3 * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, -3, 3);
        transform.position = newPosition;
    }

    public void ResetPosition()
    {
        transform.localPosition = new Vector3(0,0,0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "TargetArea")
        {
            isinArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "TargetArea")
        {
            isinArea = false;
        }
    }
}
