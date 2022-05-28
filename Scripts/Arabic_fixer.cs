using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;

public class Arabic_fixer : MonoBehaviour
{
    public InputField text;
    public Text ArabicText;

    public Text[] Ar_texts;


    // Start is called before the first frame update
    void Start()
    {
        foreach (Text text in Ar_texts)
        {
            text.text = ArabicFixer.Fix(text.text);
        }
    }

    public void arabicFIX()
    {
        string temp = text.text;
        ArabicText.text = ArabicFixer.Fix(temp);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
