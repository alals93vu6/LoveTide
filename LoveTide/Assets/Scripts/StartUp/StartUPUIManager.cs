using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUPUIManager : MonoBehaviour
{
    [SerializeField] private GameObject configure;
    // Start is called before the first frame update
    public void ClickConfigure(bool isOpen)
    {
        configure.SetActive(isOpen);
    }
}
