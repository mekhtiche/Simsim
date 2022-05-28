using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using ArabicSupport;

public class Players_controller : MonoBehaviour
{
    public GameObject select_panel, new_player_panel, stage_panel;
    public GameObject[] players_avatars;
    public Sprite[] avatar_images;
    public InputField player_name;
    private Players_list players;
    public Toggle[] avatars;
    public int avatar_ID;
    public Button add_player_button;
    private string save_path;

    // Start is called before the first frame update
    void Start()
    {
        load_players();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void load_players()
    {
        foreach (GameObject go in players_avatars)
            go.SetActive(false);
        players = new Players_list();
        save_path = Application.persistentDataPath + "/players.txt";
        if (File.Exists(save_path))
        {
            string players_txt = File.ReadAllText(save_path);
            players = JsonUtility.FromJson<Players_list>(players_txt);
            if (players == null)
                print("File Empty");
            else if (players.players.Count == 0)
                print("List Empty");
            else
            {
                for (int i = 0; i < players.players.Count; i++)
                {
                    Player player = players.players[i];
                    players_avatars[i].SetActive(true);
                    Text t = players_avatars[i].transform.Find("Text").GetComponent<Text>();
                    players_avatars[i].GetComponent<Image>().sprite = avatar_images[player.avatar_ID];
                    t.text =ArabicFixer.Fix(player.name);
                }
                if (players.players.Count > 4)
                    add_player_button.enabled = false;
                else
                    add_player_button.enabled = true;
            }
        }
        else
            print("File Does Not Exist");
    }

    public void new_player()
    {
        new_player_panel.SetActive(true);
    }

    public void quit_new_player()
    {
        new_player_panel.SetActive(false);
    }
    public void add_player()
    {
        if (player_name.text != "")
        {
            Player p = new Player();
            p.name = player_name.text;
            p.avatar_ID = choose_avatar();
            players.players.Add(p);
            string json = JsonUtility.ToJson(players);
            File.WriteAllText(save_path, json);
            load_players();
            player_name.text = "";
            new_player_panel.SetActive(false);
            print("player added");
        }
        else
        {
            print("please add name");
        }
    }

    public void Delete_player(int ID)
    {
        players.players.RemoveAt(ID);
        string json = JsonUtility.ToJson(players);
        File.WriteAllText(save_path, json);
        load_players();
    }
    public int choose_avatar()
    {
        for (int i = 0; i < avatars.Length; i++)
        {
            Toggle t = avatars[i];
            if (t.isOn)
            {
                avatar_ID = i;
                print(avatar_ID);
            }
        }
        return avatar_ID;
    }

    public void Go_to_stage_panel(int ID)
    {
        select_panel.SetActive(false);
        stage_panel.SetActive(true);
        this.GetComponent<Stages_controller>().selected_player = players.players[ID];
        this.GetComponent<Stages_controller>().update_map();
    }

    public void back()
    {
        stage_panel.SetActive(false);
        select_panel.SetActive(true);
    }
}
[Serializable]
public class Player
{
    public string name;
    public string password;
    public int avatar_ID;
    public List<int> scores = new List<int>();
}

[Serializable]
public class Players_list
{
    public string comment;
    public List<Player> players = new List<Player>();
}