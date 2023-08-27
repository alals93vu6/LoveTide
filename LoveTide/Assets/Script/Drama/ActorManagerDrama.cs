using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorManagerDrama : MonoBehaviour
{
    [SerializeField]public Image[] theActor;
    [SerializeField]public ActorScObj[] actorImage;
    [SerializeField] private DialogData dialog;
    
    
    //todo dia判斷是否改變動作，

    [ContextMenu("ReturnTest")]
    public void AAA()
    {
        Debug.Log("VAR");
        var textNumber = ChickFace(0);
        Debug.Log(textNumber);
    }

    public void OnStart(DialogData diadata)
    {
        dialog = diadata;
    }
    
    public void ActorCtrl(int theTargetActor)
    {
        ChangeActorFace(theTargetActor,1);
        MoveActorLocation(theTargetActor,1);
    }

    // Start is called before the first frame update
    public void ChangeActorFace(int targetActor,int targetFace)
    {
        theActor[targetActor].sprite = actorImage[targetActor].ActorStandingDrawing[targetFace];
        
    }

    public void MoveActorLocation(int targetActor,int targetLocation)
    {
        theActor[targetActor].GetComponent<ActorLocationCtrl>().StayTarget = targetLocation;
    }
    
    private int ChickFace(int faceNumber)
    {
        switch (dialog.dialogDataDetails[FindObjectOfType<TextBoxDrama>().textNumber].actorFace)
        {
            case Face.nothiog: faceNumber = 0; break;
            case Face.normal: faceNumber = 1; break;
            case Face.haapy: faceNumber = 2; break;
            case Face.blush: faceNumber = 3; break;
            case Face.cry: faceNumber = 4; break;
            case Face.hrony: faceNumber = 5; break;
            case Face.angry: faceNumber = 6; break;
            case Face.anxious: faceNumber = 6; break;
        }
        return faceNumber;
    }
}
