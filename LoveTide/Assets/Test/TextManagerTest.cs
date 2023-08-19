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
    [SerializeField]
    public class TextClass
    {
        public string[] TheText;
    }
    [SerializeField] private TextClass getTextDate;

    [SerializeField] private bool Isover = true;

    [SerializeField] private bool StopLoop;
    // Start is called before the first frame update
    void Start()
    {
        TextDateLoad();
        StartCoroutine(DisplayTextWithTypingEffect(false));
    }

    // Update is called once per frame
    void Update()
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

    private void TextDateLoad()
    {
        string filepath = Path.Combine("Assets/Test/TextAssetsTest/TestAText.json");
        StreamReader textfile = new StreamReader(filepath);
        string stringText = textfile.ReadToEnd();
        getTextDate = JsonUtility.FromJson<TextClass>(stringText);
        textfile.Close();
    }

    private IEnumerator DisplayTextWithTypingEffect(bool OnWork)
    {
        Isover = true;
        if ( getTextDate.TheText.Length > TextNumber)
        {
            string targetText = getTextDate.TheText[TextNumber];
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
    
    private void ShowFullText()
    {
        showText.text = getTextDate.TheText[TextNumber];
    }

    private void NextText()
    {
        if (TextNumber < getTextDate.TheText.Length - 1)
        {
            StopLoop = false;
            TextNumber++;
            StartCoroutine(DisplayTextWithTypingEffect(false));
        }
    }

    private void DownText()
    {
        StopLoop = true;
        StartCoroutine(DisplayTextWithTypingEffect(true));
    }
}
