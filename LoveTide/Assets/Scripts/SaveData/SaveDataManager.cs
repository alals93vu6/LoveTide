using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    [SerializeField] public NumericalRecords playerData;
    // Start is called before the first frame update
    void Start()
    {
        playerData.OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
