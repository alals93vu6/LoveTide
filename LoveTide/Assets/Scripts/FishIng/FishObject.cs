using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "FishObject/FishObject")]
public class FishObject : ScriptableObject
{
    public List<FishSpecies> theFish;
}

[System.Serializable]
public class FishSpecies
{
    public string fishName;
    public Sprite fishImage;
    public string fishIntroduction;
    public string fishPrice;
}
