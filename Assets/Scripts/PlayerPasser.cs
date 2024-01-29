using Microsoft.AspNetCore.SignalR.Client;
using ShogiServer.WebApi.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPasser : MonoBehaviour
{
    public static PlayerPasser instance;
    public HubConnection connection1 = null!;
    public ServerPlayer hostPlayer = null!;
    public Player Winner;
    public Player player1 = new HumanPlayer("player1", true);
    public Player player2 = new HumanPlayer("player2", true);
    public GameDTO game = null!;
    public Dictionary<string,string> configuration = new Dictionary<string, string>() {
        { "ServerUrl","https://shogiserverwebapi20231230201349.azurewebsites.net/shogi-hub" },
        { "MaxTimeOnMove","5000"},
        {"StaticCamera","true" }
    };

    void Awake()
    {
        WriteOrReadConfiguration();
        instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void WriteOrReadConfiguration()
    {
        string path = Application.persistentDataPath + "/Shogi.config";
        UnityEngine.Debug.Log("config path: " + path);
        if(!File.Exists(path))
        {
            using(FileStream f = File.Create(path))
            {
                JsonSerializer.Serialize(f, configuration);
            }
        }
        else
        {
            var json = File.ReadAllText(path);
            configuration = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }
        string logConfig = "Configuration Loaded entrees are:\n";
        foreach(var e in configuration)
        {
            logConfig += $"{e.Key} : {e.Value}\n";
        }
        UnityEngine.Debug.Log(logConfig);
    }
}
