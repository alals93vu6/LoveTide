using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtyTrickObject : MonoBehaviour
{
    public void TimeIsDown()
    {
        this.gameObject.SetActive(false);
    }
}
