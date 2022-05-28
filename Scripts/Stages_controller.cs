using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class Stages_controller : MonoBehaviour
{
    public Player selected_player;
    public Button[] stages;
    public GameObject[] games;
    
    public void update_map()
    {
        for (int i = 0; i < selected_player.scores.Count + 1; i++)
        {
            stages[i].interactable = true;
        }
    }

    public void go_to_game1()
    {
        games[0].GetComponent<Game_1>().player = selected_player;
        games[0].SetActive(true);
    }

    public void go_to_game2()
    {
        games[1].GetComponent<Game_2>().player = selected_player;
        games[1].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
