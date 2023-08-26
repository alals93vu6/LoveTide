using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Speaker
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

public enum Face
{
    haapy,cry,hrony,fuckoff,
}

[CreateAssetMenu(fileName = "DialogData", menuName = "ScriptableObject/DialogData")]
public class DialogData : ScriptableObject
{
    public float gapTimer;
    public List<DialogDataDetail> dialogDataDetails;
}

[System.Serializable]
public class DialogDataDetail
{
    public string ID;
    public List<SentenceDetail> sentenceDetails;
}



[System.Serializable]
public class SentenceDetail
{
    public Speaker speaker;
    public Face actorFace;
    public string sentence;
    public UnityEvent talkEvent;
}