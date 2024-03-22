using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SexyUIManager : MonoBehaviour
{
    [SerializeField] public GameObject[] actorButton;
    [SerializeField] public Button[] idleButtons;
    [SerializeField] public Button[] handHobButtons;
    [SerializeField] public Button[] sexyButtons;
    [SerializeField] public Slider[] motionCtrlSlider;
    // Start is called before the first frame update
    void Start()
    {
        SetButtonDisplay(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetButtonLimitation(int targetButton,bool isOpen)
    {
        switch (targetButton)
        {
            case 1:
                for (int i = 0; i < idleButtons.Length; i++)
                {
                    idleButtons[i].interactable = isOpen;
                }
                break;
            case 2:
                for (int i = 0; i < handHobButtons.Length; i++)
                {
                    handHobButtons[i].interactable = isOpen;
                }
                break;
            case 3:
                for (int i = 0; i < sexyButtons.Length; i++)
                {
                    sexyButtons[i].interactable = isOpen;
                }
                break;
        }
    }

    public void SetSliderLimitation(bool isOpen)
    {
        for (int i = 0; i < motionCtrlSlider.Length; i++)
        {
            motionCtrlSlider[i].interactable = isOpen;
        }
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
