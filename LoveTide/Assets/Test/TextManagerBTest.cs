using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using Task = System.Threading.Tasks.Task;

public class TextManagerBTest : MonoBehaviour
{
    [SerializeField] private DialogTestData logDate;
    [SerializeField] private Text LogText;
    [SerializeField] private Text nameText;

    [ContextMenu("ActorTest")]
    public void TestLOG()
    {
        StartCoroutine(startDialog());
        
    }

    private void Start()
    {
        StartCoroutine(startDialog());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(startDialog());
        }
    }

    IEnumerator startDialog()
    {
        var dialogData = Resources.Load<DialogData>("TestAObj");
        //var dialogDataList = GetDialogDataDetail("1-1", dialogData);

        //LogText.text = dialogDataList.sentenceDetails[0].sentence;
        //foreach (var sentenceDetail in dialogDataList.sentenceDetails)
        {
            //SetDialogTalkName(sentenceDetail.speaker);
            //SetDialogText(sentenceDetail.sentence);
            
            //DetectException(sentenceDetail.sentence);

            yield return null;

        }

        //SetDialogTextActive(false);
        //yield return null;
    }
    /*
    public DialogDataDetail GetDialogDataDetail(string _ID , DialogData dialogData)
    {
        //return dialogData.dialogDataDetails.Where(t => t.ID == _ID).FirstOrDefault();
        //return dialogData.dialogDataDetails.Where(t => t.ID == _ID).FirstOrDefault();
    }
    
    void SetDialogTextActive(bool active)
    {
        //dialogTextParent.SetActive(active);
    }
    
    async void SetDialogText(string t)
    {
        LogText.text = "";
        //dialogTextParent.gameObject.SetActive(true);
        for (int i = 0; i < t.Length; i++)
        {
            LogText.text += t[i];
            await Task.Delay(50);
            //yield return new WaitForSeconds(0.02f);
            Debug.Log("A");
            
        }
        //LogText.text = t;
    }
    
    void SetDialogTalkName(Speaker speaker)
    {
        switch (speaker)
        {
            case Speaker.Player:nameText.text = "玩家"; break;
            case Speaker.Girlfriend:nameText.text = "女主角"; break;
            case Speaker.Boyfriend:nameText.text = "苦主"; break;
            case Speaker.Steve:nameText.text = "史帝夫"; break;
            case Speaker.PoliceA:nameText.text = "警察A"; break;
            case Speaker.PoliceB:nameText.text = "警察B"; break;
            case Speaker.PassersbyA:nameText.text = "路人甲"; break;
            case Speaker.PassersbyB:nameText.text = "路人乙"; break;
        }
    }*/
}
