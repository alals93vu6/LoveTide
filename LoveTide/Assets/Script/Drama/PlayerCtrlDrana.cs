using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrlDrana : MonoBehaviour
{
    [Header("物件")]
    [SerializeField] private TextBoxDrama texBox;
    [SerializeField] private ActorManagerDrama actorCtrl;
    [SerializeField] private CGDisplay CGDisplay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerClick();
    }

    private void PlayerClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            if (texBox.isover)
            {
                texBox.DownText();
            }
            else
            {
                texBox.NextText();
            }
        }
    }
}
