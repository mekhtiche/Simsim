using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;

public class Game_1 : MonoBehaviour
{
    public GameObject canvas;
    public Player player;
    public Image[] rubets;
    public Text[] texts;
    private int ID, word_ID;
    public AudioClip[] clips;
    public string[] words;
    private int counter = 0;


    private void Start()
    {
        words=new string[]{"با","بو","بي"};
    }

    private void FixedUpdate()
    {
        if (!rubets[0].GetComponent<Animator>().GetBool("Go") &
            !rubets[1].GetComponent<Animator>().GetBool("Go") &
            !rubets[2].GetComponent<Animator>().GetBool("Go"))
        {
            ID = Random.Range(0, 3);
            word_ID  = Random.Range(0, 3);
            texts[ID].text = ArabicFixer.Fix(words[word_ID]);
            rubets[ID].GetComponent<Animator>().SetBool("Go", true);
        }

        if(!rubets[0].GetComponent< AudioSource>().isPlaying&
           !rubets[1].GetComponent<AudioSource>().isPlaying &
           !rubets[2].GetComponent<AudioSource>().isPlaying)
        {
            rubets[0].GetComponent<Animator>().speed = 1.0f;
            rubets[1].GetComponent<Animator>().speed = 1.0f;
            rubets[2].GetComponent<Animator>().speed = 1.0f;
        }
    }

    public void onPress(int ID)
    {
        rubets[ID].GetComponent<Animator>().speed = 0.0f;
        rubets[ID].GetComponent<AudioSource>().PlayOneShot(clips[word_ID]);
        counter+=1;
        if(counter>9){
            player.scores.Add(counter);
            canvas.GetComponent<Stages_controller>().update_map();
            this.gameObject.SetActive(false);
        }
    }
    public void back()
    {
        this.gameObject.SetActive(false);
    }
}