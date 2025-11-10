using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogDataManager : MonoBehaviour
{
    [SerializeField] public DialogDataDetected dialogDataDetected;
    [SerializeField] public List<GameDiaData> DiaDataList;
    [SerializeField] public TextAsset testJson;

    public void OnLoadDialogData(int progress)
    {
        var route =  dialogDataDetected.DiaLogListDetected(progress);
        testJson = Resources.Load<TextAsset>(route);
        OnGetDiaLog(route);
        /*
        for (int i = 0; i < DiaDataList.Count; i++)
        {
            Debug.Log(DiaDataList[i].ActorFace);
        }*/
    }

    public void OnGetDiaLog(string resultJson)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(resultJson);

        var jsonData = jsonFile.text;

        DiaDataList = JsonConvert.DeserializeObject<List<GameDiaData>>(jsonData);
        /*for (int i = 0; i < DiaDataList.Count; i++)
        {
            Debug.Log(DiaDataList[i].ActorName);
        }*/
    }
}

