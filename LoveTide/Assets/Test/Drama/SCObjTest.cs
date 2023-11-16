using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ActorTest
{
    Player,
    Girlfriend,
    Boyfriend,
    Steve,
    PoliceA,
    PoliceB,
    PassersbyA,
    PassersbyB,
    
}

public enum ActorFaceTest
{
    normal,//一般
    happy,//快樂
    blush,//害羞
    hrony,//慾火焚身
    upset,//傷心
    angry,//憤怒
    anxious,//焦慮
    
}

[CreateAssetMenu(fileName = "DialogDataTest", menuName = "ScriptableObject/DialogDataTest")]
public class DialogTestData : ScriptableObject
{
    public Sprite[] eventCG;
    public int CGOrder;
    public List<DialogDataDetailTest> dialogDataDetailstest;
}

[System.Serializable]
public class DialogDataDetailTest
{
    public ActorTest actor;
    public ActorFaceTest actorFace;
    public int actorLocation;
    public string sentence;
    public bool changeDisplayCG;
    public bool switchCG;
    public UnityEvent talkEvent;
}