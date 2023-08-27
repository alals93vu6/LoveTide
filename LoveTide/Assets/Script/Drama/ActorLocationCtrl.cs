using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorLocationCtrl : MonoBehaviour
{
    public GameObject[] StayLocation;
    public int StayTarget;
    
    void Update()
    {
        this.transform.position = Vector3.Lerp(this.transform.position,
            new Vector3(StayLocation[StayTarget].transform.position.x, this.transform.position.y, this.transform.position.z), 0.03f);
    }
}
