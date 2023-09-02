using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorManagerDrama : MonoBehaviour
{
    [SerializeField]public Image[] theActor;
    [SerializeField]public ActorScObj[] actorImage;
    [SerializeField]private DialogData dialog;
    [SerializeField] private int idleActor = 0;
    
    
    //todo dia判斷是否改變動作，

    [ContextMenu("ReturnTest")]
    public void AAA()
    {
        Debug.Log("VAR");
        var textNumber = ChickFace(0);
        Debug.Log(textNumber);
    }

    public void OnStart(DialogData diadata,int actorLocation)
    {
        dialog = diadata;
        ActorCtrl(actorLocation);
    }
    
    public void ActorCtrl(int theActorLocation)
    {   
        ChickActor();
        if (theActorLocation != 0)
        {
            MoveActorLocation(idleActor,theActorLocation);
        }
        if (dialog.plotOptionsList[0].dialogDataDetails[FindObjectOfType<TextBoxDrama>().textNumber].actorFace != Face.nothiog)
        {
            ChangeActorFace(idleActor,ChickFace(0));
        }
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
        switch (dialog.plotOptionsList[0].dialogDataDetails[FindObjectOfType<TextBoxDrama>().textNumber].actorFace)
        {
            case Face.normal: faceNumber = 0; break;
            case Face.haapy: faceNumber = 1; break;
            case Face.blush: faceNumber = 2; break;
            case Face.cry: faceNumber = 3; break;
            case Face.hrony: faceNumber = 4; break;
            case Face.angry: faceNumber = 5; break;
            case Face.anxious: faceNumber = 6; break;
            case Face.sad: faceNumber = 7; break;
        }
        //Debug.Log("Face"+faceNumber);
        return faceNumber;
    }

    private void ChickActor()
    {
        switch (dialog.plotOptionsList[0].dialogDataDetails[FindObjectOfType<TextBoxDrama>().textNumber].speaker)
        {
            case Speaker.Player : idleActor = 0; break;
            case Speaker.GirlFriend : idleActor = 1; break;
            case Speaker.BoyFriend : idleActor = 2; break;
            case Speaker.Steve : idleActor = 3; break;
            case Speaker.PoliceA : idleActor = 4; break;
            case Speaker.PoliceB : idleActor = 5; break;
            case Speaker.PassersbyA : idleActor = 6; break;
            case Speaker.PassersbyB : idleActor = 7; break;
            
        }
    }

}
