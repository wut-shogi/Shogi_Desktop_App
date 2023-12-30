using Microsoft.AspNetCore.SignalR.Client;
using ShogiServer.WebApi.Model;
using System;

class RemotePlayer : Player
{
    
    public RemotePlayer(string name, bool positiveZMovement) : base(name, positiveZMovement)
    {
        
    }

    public override string MakeMove(string fen)
    {
        try
        {
            PlayerPasser.instance.connection1.InvokeAsync("MakeMove", new MakeMoveRequest(PlayerPasser.instance.game.Id, PlayerPasser.instance.hostPlayer.Token, fen));
        }
        catch (Exception e)
        {
        }
        return "";
    }
}