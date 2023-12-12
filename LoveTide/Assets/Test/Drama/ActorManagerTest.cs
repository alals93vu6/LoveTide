using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ActorManagerTest : MonoBehaviour
{
    public Image[] TheActor;
    public ActorScObj[] ActorImage;
    [SerializeField]private DialogData dialog;
    [SerializeField] private int idleActor = 0;
    // Start is called before the first frame update
    public void OnStart(DialogData diadata)
    {
        dialog = diadata;
        var startApparel = 0;
        if (FindObjectOfType<TimeManagerTest>().vacation){startApparel = 1;}else{startApparel = 0;}
        ChangeFace(startApparel,0);
        TheActor[0].gameObject.SetActive(false);
    }

    private void Start()
    {
        
    }

    public void ActorCtrl()
    {
        ChickActor();
        if (dialog.plotOptionsList[FindObjectOfType<TextBoxTestPlaying>().listSerial].dialogDataDetails[FindObjectOfType<TextBoxTestPlaying>().textNumber].actorFace != Face.nothiog)
        {
            ChangeFace(idleActor,ChickFace(0));
        }
    }
    
    public void ChangeFace(int targetActor,int targetFace)
    {
        //Debug.Log("ChangeFace");
        TheActor[targetActor].sprite = ActorImage[targetActor].ActorStandingDrawing[targetFace];
    }

    public void MoveActorLocation(int targetActor,int targetLocation)
    {
        TheActor[targetActor].GetComponent<ActorLocationCtrl>().StayTarget = targetLocation;
    }
    
    
    private int ChickFace(int faceNumber)
    {
        switch (dialog.plotOptionsList[FindObjectOfType<TextBoxTestPlaying>().listSerial].dialogDataDetails[FindObjectOfType<TextBoxTestPlaying>().textNumber].actorFace)
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

    private void ChickActor()
    {
        if (FindObjectOfType<TimeManagerTest>().vacation)
        {
            //Debug.Log("便服");
            switch (dialog.plotOptionsList[FindObjectOfType<TextBoxTestPlaying>().listSerial].dialogDataDetails[FindObjectOfType<TextBoxTestPlaying>().textNumber].speaker)
            {
                case Speaker.Player : idleActor = 1; break;
                case Speaker.GirlFriend : idleActor = 1; break;
                case Speaker.Chorus : idleActor = 1; break;
            }
        }
        else
        {
            //Debug.Log("制服");
            switch (dialog.plotOptionsList[FindObjectOfType<TextBoxTestPlaying>().listSerial].dialogDataDetails[FindObjectOfType<TextBoxTestPlaying>().textNumber].speaker)
            {
                case Speaker.Player : idleActor = 0; break;
                case Speaker.GirlFriend : idleActor = 0; break;
                case Speaker.Chorus : idleActor = 0; break;
            }
        }

        
    }
    
    /* case Speaker.BoyFriend : idleActor = 2; break;
            case Speaker.Steve : idleActor = 3; break;
            case Speaker.PoliceA : idleActor = 4; break;
            case Speaker.PoliceB : idleActor = 5; break;
            case Speaker.PassersbyA : idleActor = 6; break;
            case Speaker.PassersbyB : idleActor = 7; break;
            */
}
