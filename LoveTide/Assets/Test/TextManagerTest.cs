using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextManagerTest : MonoBehaviour
{
    [SerializeField] public Text showText;
    [SerializeField] public float letterSpeed = 0.02f;
    [SerializeField] public int TextNumber = 0;
    [SerializeField] public DialogTestData DiaLog;
    [SerializeField] public ActorManagerTest actorManager;
    [SerializeField] public string[] getTextDate;
    [SerializeField] private Text nameText;
    [SerializeField] private GameObject gameCG;
    
    [SerializeField] private bool Isover = true;

    [SerializeField] private bool StopLoop;
    // Start is called before the first frame update
    void Start()
    {
        DiaLog = Resources.Load<DialogTestData>("DialogDataTestB");
        gameCG.SetActive(false);
        DiaLog.CGOrder = 0;
        TextDateLoad();
        ChickName();
        StartCoroutine(DisplayTextWithTypingEffect(false));
    }

    // Update is called once per frame
    void Update()
    {
        TextCtrl();
        /*if (Input.GetKeyDown(KeyCode.T))
        {
            
        }*/
    }

    private void TextDateLoad()
    {
        var arraySize = DiaLog.dialogDataDetailstest.Count;
        Debug.Log(arraySize);
        //getTextDate.Length = arraySize;
        for (int i = 0; i < DiaLog.dialogDataDetailstest.Count; i++)
        {
            getTextDate[i] = DiaLog.dialogDataDetailstest[i].sentence;
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
        switch (DiaLog.dialogDataDetailstest[TextNumber].actor)
        {
           case ActorTest.Player: nameText.text = "玩家"; break;
           case ActorTest.Girlfriend: nameText.text = "織那久菜子"; break;
           case ActorTest.Boyfriend: nameText.text = "苦主"; break;
           case ActorTest.Steve: nameText.text = "史帝夫"; break;
           case ActorTest.PoliceA: nameText.text = "警察A"; break;
           case ActorTest.PoliceB: nameText.text = "警察B"; break;
           case ActorTest.PassersbyA: nameText.text = "路人A"; break;
           case ActorTest.PassersbyB: nameText.text = "路人B"; break;
        }
    }

    private void NextText()
    {
        if (TextNumber < DiaLog.dialogDataDetailstest.Count - 1)
        {
            StopLoop = false;
            TextNumber++;
            ChangeFace();
            ChickName();
            CGCtrl();
            StartCoroutine(DisplayTextWithTypingEffect(false));
        }
    }

    private void CGCtrl()
    {
        if (DiaLog.dialogDataDetailstest[TextNumber].changeDisplayCG)
        {
            if (gameCG.activeSelf)
            {
                gameCG.SetActive(false);
            }
            else
            {
                gameCG.SetActive(true);
            }
        }

        if (DiaLog.dialogDataDetailstest[TextNumber].switchCG)
        {
            DiaLog.CGOrder++;
        }

        if (gameCG.activeSelf)
        {
            gameCG.GetComponent<Image>().sprite = DiaLog.eventCG[DiaLog.CGOrder];
        }
    }

    private void ChangeFace()
    {
        var targetActorInt =0;
        var targetFaceInt =0;
        switch (DiaLog.dialogDataDetailstest[TextNumber].actor)
        {
            case ActorTest.Player: targetActorInt = 0; break;
            case ActorTest.Girlfriend: targetActorInt = 1; break;
            case ActorTest.Boyfriend: targetActorInt = 2; break;
            case ActorTest.Steve: targetActorInt = 3; break;
            case ActorTest.PoliceA: targetActorInt = 4; break;
            case ActorTest.PoliceB: targetActorInt = 5; break;
            case ActorTest.PassersbyA: targetActorInt = 6; break;
            case ActorTest.PassersbyB: targetActorInt = 7; break;
        }
        switch (DiaLog.dialogDataDetailstest[TextNumber].actorFace)
        {
            case ActorFaceTest.normal: targetFaceInt = 0; break;
            case ActorFaceTest.happy: targetFaceInt = 1; break;
            case ActorFaceTest.blush: targetFaceInt = 2; break;
            case ActorFaceTest.hrony: targetFaceInt = 3; break;
            case ActorFaceTest.upset: targetFaceInt = 4; break;
            case ActorFaceTest.angry: targetFaceInt = 5; break;
            case ActorFaceTest.anxious: targetFaceInt = 6; break;
        }
        
        actorManager.ChangeA(targetActorInt,targetFaceInt);
        actorManager.MoveActorLocation(targetActorInt,DiaLog.dialogDataDetailstest[TextNumber].actorLocation);
    }

    private void DownText()
    {
        StopLoop = true;
        StartCoroutine(DisplayTextWithTypingEffect(true));
    }
}
