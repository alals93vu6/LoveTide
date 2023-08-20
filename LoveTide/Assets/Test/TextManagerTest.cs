using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TextManagerTest : MonoBehaviour
{
    [SerializeField] public Text showText;
    [SerializeField] public float letterSpeed = 0.02f;
    [SerializeField] public int TextNumber = 0;
    [SerializeField] public DialogTestData DiaLog;
    [SerializeField] public string[] getTextDate;
    [SerializeField] private Text nameText;
    
    [SerializeField] private bool Isover = true;

    [SerializeField] private bool StopLoop;
    // Start is called before the first frame update
    void Start()
    {
        TextDateLoad();
        ChickName();
        StartCoroutine(DisplayTextWithTypingEffect(false));
    }

    // Update is called once per frame
    void Update()
    {
        TextCtrl();
    }

    private void TextDateLoad()
    {
        var arraySize = DiaLog.dialogDataDetails.Count;
        Debug.Log(arraySize);
        //getTextDate.Length = arraySize;
        for (int i = 0; i < DiaLog.dialogDataDetails.Count; i++)
        {
            getTextDate[i] = DiaLog.dialogDataDetails[i].sentence;
        }
        
        /*
        string filepath = Path.Combine("Assets/Test/TextAssetsTest/TestAText.json");
        StreamReader textfile = new StreamReader(filepath);
        string stringText = textfile.ReadToEnd();
        getTextDate = JsonUtility.FromJson<TextClass>(stringText);
        textfile.Close();
        */
    }

    private IEnumerator DisplayTextWithTypingEffect(bool OnWork)
    {
        Isover = true;
        if ( getTextDate.Length > TextNumber)
        {
            string targetText = getTextDate[TextNumber];
            showText.text = "";

            if (!OnWork)
            {
                for (int i = 0; i < targetText.Length; i++)
                {
                    if (StopLoop == true)
                    {
                        break;
                    }
                    showText.text += targetText[i];
                    yield return new WaitForSeconds(letterSpeed);
                    //Debug.Log("A");
                }
                Isover = false;
            }
            else
            {
                //Debug.Log("B");
                showText.text = "";
                showText.text = targetText;
                Isover = false;
            }
            //letterSpeed = 0.02f;
        }
    }
    
    private void TextCtrl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Isover)
            {
                DownText();
            }
            else
            {
                NextText();
            }
        }
    }

    private void ChickName()
    {
        switch (DiaLog.dialogDataDetails[TextNumber].actor)
        {
           case Actor.Player: nameText.text = "玩家"; break;
           case Actor.Girlfriend: nameText.text = "織那久菜子"; break;
           case Actor.Boyfriend: nameText.text = "苦主"; break;
           case Actor.Steve: nameText.text = "史帝夫"; break;
           case Actor.PoliceA: nameText.text = "警察A"; break;
           case Actor.PoliceB: nameText.text = "警察B"; break;
           case Actor.PassersbyA: nameText.text = "路人A"; break;
           case Actor.PassersbyB: nameText.text = "路人B"; break;
        }
    }

    private void NextText()
    {
        if (TextNumber < DiaLog.dialogDataDetails.Count - 1)
        {
            StopLoop = false;
            TextNumber++;
            ChickName();
            StartCoroutine(DisplayTextWithTypingEffect(false));
        }
    }

    private void DownText()
    {
        StopLoop = true;
        StartCoroutine(DisplayTextWithTypingEffect(true));
    }
}
