using Microsoft.AspNetCore.SignalR.Client;
using ShogiServer.WebApi.Model;
using System;

class HumanPlayer : Player
{
    public bool is_online = false;
    
    public HumanPlayer(string name, bool positiveZMovement) : base(name, positiveZMovement)
    {

    }

    public override string MakeMove(string fen)
    {
       

        return "";
    }
}