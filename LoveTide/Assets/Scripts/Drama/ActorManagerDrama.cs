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
    [SerializeField] private int targetDialog;
    [SerializeField] private int oldActor;
    
    
    //todo dia判斷是否改變動作，

    [ContextMenu("ReturnTest")]
    public void AAA()
    {
        Debug.Log("VAR");
        var textNumber = ChickFace(0);
        Debug.Log(textNumber);
    }

    public void OnStart(DialogData diadata,int actorLocation,int targetNumber)
    {
        dialog = diadata;
        targetDialog = targetNumber;
        ActorCtrl(actorLocation);
    }
    
    public void ActorCtrl(int theActorLocation)
    {   
        ChickActor();
        if (theActorLocation != 0)
        {
            MoveActorLocation(idleActor,theActorLocation);
        }
        
        /*問題*/
        if (dialog.plotOptionsList[targetDialog].dialogDataDetails[FindObjectOfType<TextBoxDrama>().textNumber].actorFace != Face.nothiog)
        {
            ChangeActorFace(idleActor,ChickFace(0));
        }
    }

    // Start is called before the first frame update
    public void ChangeActorFace(int targetActor,int targetFace)
    {
        //Debug.Log(targetFace);
        theActor[targetActor].sprite = actorImage[ChickImage(0)].ActorStandingDrawing[targetFace];
    }

    public void MoveActorLocation(int targetActor,int targetLocation)
    {
        theActor[targetActor].GetComponent<ActorLocationCtrl>().StayTarget = targetLocation;
    }
    
    private int ChickFace(int faceNumber)
    {
        switch (dialog.plotOptionsList[targetDialog].dialogDataDetails[FindObjectOfType<TextBoxDrama>().textNumber].actorFace)
        {
            case Face.normal: faceNumber = 0; break;
            case Face.haapy: faceNumber = 1; break;
            case Face.blush: faceNumber = 2; break;
            case Face.cry: faceNumber = 3; break;
            case Face.hrony: faceNumber = 4; break;
            case Face.angry: faceNumber = 5; break;
            case Face.anxious: faceNumber = 6; break;
            case Face.sad: faceNumber = 7; break;
            case Face.superise: faceNumber = 8; break;
            case Face.alaise: faceNumber = 9; break;
        }
        //Debug.Log("Face"+faceNumber);
        return faceNumber;
    }
    
    private int ChickImage(int imageNumber)
    {
        switch (dialog.plotOptionsList[targetDialog].dialogDataDetails[FindObjectOfType<TextBoxDrama>().textNumber].speaker)
        {
            case Speaker.Ibid : imageNumber = oldActor; break;
            case Speaker.Player : imageNumber = oldActor; break;
            case Speaker.Chorus : imageNumber = oldActor; break;
            case Speaker.GirlFriend : imageNumber = 1;
                oldActor = imageNumber; break;
            case Speaker.GirlFriendDormitory : imageNumber = 8;
                oldActor = imageNumber; break; break;
            case Speaker.GirlFriendFormal : imageNumber = 9;
                oldActor = imageNumber; break; break;
            case Speaker.GirlFriendNude : imageNumber = 10;
                oldActor = imageNumber; break; break;
            case Speaker.BoyFriend : imageNumber = 2;
                oldActor = imageNumber; break; break;
            case Speaker.Steve : imageNumber = 3;
                oldActor = imageNumber; break; break;
            case Speaker.PoliceA : imageNumber = 4;
                oldActor = imageNumber; break; break;
            case Speaker.PoliceB : imageNumber = 5;
                oldActor = imageNumber; break; break;
            case Speaker.PassersbyA : imageNumber = 6;
                oldActor = imageNumber; break; break;
            case Speaker.PassersbyB : imageNumber = 7;
                oldActor = imageNumber; break; break;
            case Speaker.TavernBoss : imageNumber = 7;
                oldActor = imageNumber; break; break;
        }
        //Debug.Log("Face"+imageNumber);
        return imageNumber;
    }

    private void ChickActor()
    {
        switch (dialog.plotOptionsList[targetDialog].dialogDataDetails[FindObjectOfType<TextBoxDrama>().textNumber].speaker)
        {
            case Speaker.Player : idleActor = 1; break;
            case Speaker.Chorus : idleActor = 1; break;
            case Speaker.GirlFriend : idleActor = 1; break;
            case Speaker.GirlFriendDormitory : idleActor = 1; break;
            case Speaker.GirlFriendFormal : idleActor = 1; break;
            case Speaker.GirlFriendNude : idleActor = 1; break;
            case Speaker.BoyFriend : idleActor = 2; break;
            case Speaker.Steve : idleActor = 3; break;
            case Speaker.PoliceA : idleActor = 4; break;
            case Speaker.PoliceB : idleActor = 5; break;
            case Speaker.PassersbyA : idleActor = 6; break;
            case Speaker.PassersbyB : idleActor = 7; break;
            case Speaker.TavernBoss : idleActor = 7; break;
        }
    }
}
