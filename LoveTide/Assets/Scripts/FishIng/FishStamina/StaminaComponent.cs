using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaComponent : MonoBehaviour
{
    [SerializeField] public Image staminaImage;
    [SerializeField] public Text staminaText;
    [SerializeField] public Animator staminaAnimator;
    [SerializeField] public Color[] staminaColor;
    // Start is called before the first frame update
    void Start()
    {
        staminaAnimator = GetComponent<Animator>();
    }
    
    public void DisplayFishStaminaComponent(float proportionsNumber,float displayNumber)
    {
        float getNumber = displayNumber / proportionsNumber;
        staminaImage.fillAmount = getNumber;
        int percentage = Mathf.RoundToInt(getNumber * 100);
        staminaText.text = percentage + " %";
        StaminaColorDetected(getNumber);
    }

    private void StaminaColorDetected(float theReduction)
    {
        if (theReduction <= 0.45f)
        {
            staminaImage.color = Color.Lerp(staminaImage.color,staminaColor[1],0.01f);
        }
        else
        {
            staminaImage.color = Color.Lerp(staminaImage.color,staminaColor[0],0.01f);
        }
    }
}
