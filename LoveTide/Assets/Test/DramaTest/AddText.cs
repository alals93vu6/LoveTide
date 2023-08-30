using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AddText : MonoBehaviour
{
    [SerializeField] public string[] addText;
    [SerializeField] public string TextJson;
    
    [SerializeField]
    public class AddTextClass
    {
        public string[] TheText;
    }
    // Start is called before the first frame update
    void Start()
    {
        var TheClass = new AddTextClass();
        TheClass.TheText = new string[addText.Length];
        for (int i = 0; i < addText.Length; i++)
        {
            TheClass.TheText[i] = addText[i];
        }
        TextJson = JsonUtility.ToJson(TheClass);
        string filePath = Path.Combine("Assets/Test/TextAssetsTest/TestAText.json");
        StreamWriter file = new StreamWriter(filePath);
        file.Write(TextJson);
        file.Close();
    }

    
}
