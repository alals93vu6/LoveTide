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
    [SerializeField] public Image[] playerStatus;
    [SerializeField] public Image[] girlStatus;
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

    public void PlayerStatusUIDisplay(float showStaminaNumber,float showDelightNumber)
    {
        playerStatus[0].fillAmount = Mathf.Lerp(playerStatus[0].fillAmount, showStaminaNumber, 0.02f);
        playerStatus[1].fillAmount = Mathf.Lerp(playerStatus[1].fillAmount, showDelightNumber, 0.01f);
    }
    
    public void GirlStatusUIDisplay(float showStaminaNumber,float showDelightNumber)
    {
        girlStatus[0].fillAmount = Mathf.Lerp(girlStatus[0].fillAmount, showStaminaNumber, 0.02f);
        girlStatus[1].fillAmount = Mathf.Lerp(girlStatus[1].fillAmount, showDelightNumber, 0.01f);
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
