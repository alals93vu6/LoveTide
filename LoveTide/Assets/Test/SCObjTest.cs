using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Actor
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

[CreateAssetMenu(fileName = "DialogDataTest", menuName = "ScriptableObject/DialogDataTest")]
public class DialogTestData : ScriptableObject
{
    public List<DialogDataDetailTest> dialogDataDetails;
}

[System.Serializable]
public class DialogDataDetailTest
{
    public Actor actor;
    public Face actorFace;
    public string sentence;
    public UnityEvent talkEvent;
}