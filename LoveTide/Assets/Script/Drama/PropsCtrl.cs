using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsCtrl : MonoBehaviour
{
    [SerializeField] public NumericalRecords numberCtrl;
    [SerializeField] private int setPropsLevel;
    // Start is called before the first frame update
    void Start()
    {
        numberCtrl.getPropsLevel = setPropsLevel;
    }
}
