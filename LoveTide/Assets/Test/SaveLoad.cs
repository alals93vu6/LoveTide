using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    [SerializeField] private int TestIntA;
    [SerializeField] private int TestIntB;
    [SerializeField] private int TestIntC;
    [SerializeField] private string json;
    
    public class Myclass
    {
        [SerializeField] public int SaveTestA;
        [SerializeField] public int SaveTestB;
        [SerializeField] public int SaveTestC;
    }
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    public void SetDate()
    {
        Myclass myObject = new Myclass();
        myObject.SaveTestA = TestIntA;
        myObject.SaveTestB = TestIntB;
        myObject.SaveTestC = TestIntC;
        json = JsonUtility.ToJson(myObject);
        Debug.Log(json);
        StreamWriter file = new StreamWriter(Path.Combine("Assets/GameJSONData", "JsonSaveDate.json"));
        file.Write(json);
        file.Close();
    }

    public void logDate()
    {
        Myclass myObject = new Myclass();
        string json = JsonUtility.ToJson(myObject);
        Debug.Log(json);
    }

    public void GetDate()
    {
        StreamReader file = new StreamReader(new FileStream(Path.Combine("Assets/GameJSONData/JsonSaveDate.json"), FileMode.Open));
        string saveDate = file.ReadToEnd();
        var getSaveDate = JsonUtility.FromJson<Myclass>(saveDate);
        Debug.Log(getSaveDate.SaveTestA + "AA");
        Debug.Log(getSaveDate.SaveTestB + "BB");
        Debug.Log(getSaveDate.SaveTestC + "CC");
    }
}
