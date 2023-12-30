using Microsoft.AspNetCore.SignalR.Client;
using ShogiServer.WebApi.Model;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PlayerItemScript : MonoBehaviour
{
    public ServerPlayer player;

    public GameObject pendingInvitePopup;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void InviteClicked()
    {
        var r = new InviteRequest(player.Nickname, PlayerPasser.instance.hostPlayer.Token);// { InvitedNickname = player.Nickname, Token = PlayerPasser.instance.hostPlayer.Token };
        PlayerPasser.instance.connection1.InvokeAsync("Invite", r);
    }
}
