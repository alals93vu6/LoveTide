using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigureSystem : MonoBehaviour
{
    [SerializeField] private GameObject quitObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadyQuitGame()
    {
        quitObject.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

}
