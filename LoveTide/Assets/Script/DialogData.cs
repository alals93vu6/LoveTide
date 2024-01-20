using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Speaker
{
    Ibid,
    Chorus,
    Player,
    GirlFriend,
    BoyFriend,
    Steve,
    PoliceA,
    PoliceB,
    PassersbyA,
    PassersbyB,
    TavernBoss,
    
}

public enum Face
{
    nothiog,normal,haapy,blush,cry,hrony,angry,anxious,sad,superise,alaise,
            //普通  開心  害羞   哭  齁逆   生氣   焦慮     傷心  驚訝     無奈
}

[CreateAssetMenu(fileName = "DialogData", menuName = "ScriptableObject/DialogData")]
public class DialogData : ScriptableObject
{
    public List<PlotOptions> plotOptionsList;
    
}

[System.Serializable]
public class PlotOptions
{
    public string talkID;
    public bool notActor;
    public Sprite[] displayCG;
    public int disPlayOrder;
    public List<DialogDataDetail> dialogDataDetails;
}

[System.Serializable]
public class DialogDataDetail
{
    public Speaker speaker;
    public Face actorFace;
    public string sentence;
    public int stayLocation;
    public bool switchCGDisplay;
    public bool switchCGImage;
    public bool needTransition;
}


