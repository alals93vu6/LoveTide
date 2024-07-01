using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateManager : MonoBehaviour
{
    [SerializeField] public GameObject[] instantiateObject;

    public void OnInstantiateObject(int objectNumber)
    {
        Instantiate(instantiateObject[objectNumber], this.transform.position, this.transform.rotation);
    }
}
