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

    [SerializeField] private bool Isover;
    // Start is called before the first frame update
    void Start()
    {
        TextDateLoad();
        StartCoroutine(DisplayTextWithTypingEffect());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextText();
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

    private IEnumerator DisplayTextWithTypingEffect()
    {
        if (getTextDate != null && getTextDate.TheText.Length > TextNumber)
        {
            Isover = true;
            string targetText = getTextDate.TheText[TextNumber];
            showText.text = "";

            for (int i = 0; i < targetText.Length; i++)
            {
                showText.text += targetText[i];
                yield return new WaitForSeconds(letterSpeed);
            }
            Isover = false;
            letterSpeed = 0.02f;
        }
    }

    private void NextText()
    {
        if (TextNumber < getTextDate.TheText.Length - 1)
        {
            if (Isover)
            {
                letterSpeed = 0.0000000000001f;
            }
            else
            {
                TextNumber++;
                StartCoroutine(DisplayTextWithTypingEffect());
            }
        }
    }
}
