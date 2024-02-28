using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SexyUIManager : MonoBehaviour
{
    [SerializeField] public GameObject[] actorButton;
    // Start is called before the first frame update
    void Start()
    {
        SetButtonDisplay(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetButtonDisplay(int displayUI)
    {
        for (int i = 0; i < actorButton.Length; i++)
        {
            actorButton[i].SetActive(false);
        }
        actorButton[displayUI].SetActive(true);
    }
}
