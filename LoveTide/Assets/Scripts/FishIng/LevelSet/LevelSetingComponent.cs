using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSetingComponent : MonoBehaviour
{
    [SerializeField] public Slider displaySlider;
    [SerializeField] public float displaySliderNumber;
    [SerializeField] public int depthLevel;
    [SerializeField] public Animator levelSetAnimator;
    // Start is called before the first frame update
    void Start()
    {
        levelSetAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        displaySlider.value = Mathf.Lerp(displaySlider.value, displaySliderNumber, 0.025f);
    }

    public void DepthLevelSet(int setLevel)
    {
        depthLevel += setLevel;
        depthLevel = Mathf.Clamp(depthLevel, 1, 3);
        switch (depthLevel)
        {
            case 1: displaySliderNumber = 1.21f; break;
            case 2: displaySliderNumber = 5.00f; break;
            case 3: displaySliderNumber = 8.88f; break;
        }
    }

}
