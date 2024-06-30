using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableDetected : MonoBehaviour
{
    [SerializeField] private GameObject yukaObject;
    [SerializeField] private bool detected;
    // Update is called once per frame
    void Update()
    {
        YukaDetected();
    }

    private void YukaDetected()
    {
        if (yukaObject.gameObject.transform.position.y >= 210)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 6;
            detected = true;
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 4;
            detected = false;
        }
    }
}
