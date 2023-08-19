using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TextManagerBTest : MonoBehaviour
{
    [SerializeField] private DialogData logDate;
    [SerializeField] private Text LogText;
    [SerializeField] private Text nameText;
    
    [ContextMenu("Test")]
    public void TestLOG()
    {
        StartCoroutine(startDialog());
    }

    IEnumerator startDialog()
    {
        var dialogData = Resources.Load<DialogData>("TestAObj");
        var dialogDataList = GetDialogDataDetail("1-1", dialogData);
        
        SetDialogTextActive(true);

        foreach (var sentenceDetail in dialogDataList.sentenceDetails)
        {
            SetDialogTalkName(sentenceDetail.speaker);
            SetDialogText(sentenceDetail.sentence);
            
            //DetectException(sentenceDetail.sentence);
            
            yield return new WaitForSeconds(1);
        }

        SetDialogTextActive(false);
        yield return null;
    }
    
    public DialogDataDetail GetDialogDataDetail(string _ID , DialogData dialogData)
    {
        return dialogData.dialogDataDetails.Where(t => t.ID == _ID).FirstOrDefault();
    }
    
    void SetDialogTextActive(bool active)
    {
        //dialogTextParent.SetActive(active);
    }
    
    void SetDialogText(string t)
    {
        //dialogTextParent.gameObject.SetActive(true);
        LogText.text = t;
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
    }
}
