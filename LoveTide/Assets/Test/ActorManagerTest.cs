using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorManagerTest : MonoBehaviour
{
    public Image[] TheActor;
    public ActorScObj[] ActorImage;
    // Start is called before the first frame update
    [ContextMenu("FaceA")]
    public void FaceA()
    {
        ChangeA(0,1);
        ChangeA(1,1);
    }
    
    [ContextMenu("FaceB")]
    public void FaceB()
    {
        ChangeA(0,2);
        ChangeA(1,2);
    }
    
    [ContextMenu("FaceC")]
    public void FaceC()
    {
        ChangeA(0,3);
        ChangeA(1,3);
    }
    
    [ContextMenu("FaceD")]
    public void FaceD()
    {
        ChangeA(0,4);
        ChangeA(1,4);
    }

    public void ChangeA(int TargetActor,int TargetFace)
    {
        TheActor[TargetActor].sprite = ActorImage[TargetActor].ActorStandingDrawing[TargetFace];
    }
}
