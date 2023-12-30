using Microsoft.AspNetCore.SignalR.Client;
using ShogiServer.WebApi.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPasser : MonoBehaviour
{
    public static PlayerPasser instance;
    public HubConnection connection1 = null!;
    public ServerPlayer hostPlayer = null!;
    public Player player1 = new HumanPlayer("player1", true);
    public Player player2 = new HumanPlayer("player2", true);
    public GameDTO game = null!;
    void Awake()
    {
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
}
