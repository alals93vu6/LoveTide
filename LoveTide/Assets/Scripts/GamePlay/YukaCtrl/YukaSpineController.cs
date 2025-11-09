using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YukaSpineController : MonoBehaviour
{
    [SerializeField] GameObject workerYuka;
    [SerializeField] GameObject normalYuka;
    [SerializeField] CharacterStartAnimation workerYukaAnimator;
    [SerializeField] CharacterStartAnimation normalYukaAnimator;

    // Start is called before the first frame update
    void Start()
    {
        workerYukaAnimator = workerYuka.GetComponent<CharacterStartAnimation>();
        normalYukaAnimator = normalYuka.GetComponent<CharacterStartAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeYukaApparel(bool isvacation,string action)
    {
        if (isvacation)
        {
            workerYuka.SetActive(false);
            normalYuka.SetActive(true);
            normalYukaAnimator.OnSetActorFace(action);
        }
        else
        {
            normalYuka.SetActive(false);
            workerYuka.SetActive(true);
            workerYukaAnimator.OnSetActorFace(action);
        }
    }

}
