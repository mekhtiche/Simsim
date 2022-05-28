using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ArabicSupport;

public class Game_2 : MonoBehaviour
{
    public Player player;
    public Text ref_word, result;
    public string speech_result;
    public string[] list_words;
    public AudioClip[] clips;

    double dist = 0;
    int ndx = 0;
    double error = 0.0f;
    double score;

    // Start is called before the first frame update
    void Start()
    {
        list_words = new string[] { "كيف.", "اسم.", "باب.", "خارج." };
        ref_word.text = ArabicFixer.Fix(list_words[ndx]);
        score = list_words.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Check_result()
    {
        var dtw = new DTW.SimpleDTW(list_words[ndx], speech_result);
        dtw.computeDTW();
        dist = dtw.getSum();
        error = dist / Math.Max(list_words[ndx].Length, speech_result.Length);

        if (error < 0.5)
        {
            result.text = "Distance " + dist + ", error is: " + error;
            score -= error;
            if(error<0.2)
                this.GetComponent<AudioSource>().PlayOneShot(clips[2]);
            else if (error < 0.3)
                this.GetComponent<AudioSource>().PlayOneShot(clips[1]);
            else 
                this.GetComponent<AudioSource>().PlayOneShot(clips[0]);
            
            if (ndx < list_words.Length - 1)
            {
                ndx += 1;
                ref_word.text = ArabicFixer.Fix(list_words[ndx]);
            }
            else
            {
                player.scores.Add((int)(score * 100));
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            result.text = ArabicFixer.Fix("حاول مرة اخرى");
            this.GetComponent<AudioSource>().PlayOneShot(clips[3]);
        }
    }

    public void back()
    {
        this.gameObject.SetActive(false);
    }
}
