using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableDetected : MonoBehaviour
{
    [SerializeField] private GameObject yukaObject;
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
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 4;
        }
    }
}
