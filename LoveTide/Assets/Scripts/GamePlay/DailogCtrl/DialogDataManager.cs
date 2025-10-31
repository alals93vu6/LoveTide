using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogDataManager : MonoBehaviour
{
    [SerializeField] public DialogDataDetected dialogDataDetected;
    [SerializeField] public List<GameDiaData> DiaDataList;
    [SerializeField] public TextAsset testJson;
    private void Start()
    {

    }

    public void OnStart(int progress)
    {
        var route =  dialogDataDetected.DiaLogListDetected(progress);
        testJson = Resources.Load<TextAsset>(route);
        OnGetDiaLog(route);
    }

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

