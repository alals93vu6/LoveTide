using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorManagerTest : MonoBehaviour
{
    public Image[] TheActor;
    public ActorScObj[] ActorImage;
    // Start is called before the first frame update
    public void ChangeA(int targetActor,int targetFace)
    {
        TheActor[targetActor].sprite = ActorImage[targetActor].ActorStandingDrawing[targetFace];
    }
}
