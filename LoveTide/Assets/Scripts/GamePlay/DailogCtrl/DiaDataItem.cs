using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public class DiaDataItem : MonoBehaviour
{
    [SerializeField] public List<GameDiaData> DiaDataList;
    [SerializeField] public TextAsset testJson;


    public void OnGetDiaLog(string resultJson)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(resultJson);

        var jsonData = jsonFile.text;

        DiaDataList = JsonConvert.DeserializeObject<List<GameDiaData>>(jsonData);
    }
}

[System.Serializable]
public class GameDiaData
{
    public int EventIndex { get; set; }
    public int DailogIndex { get; set; }
    public string ActorName { get; set; }
    public string Dailog { get; set; }
    public string ActorFace { get; set; }
}
