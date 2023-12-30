using Microsoft.AspNetCore.SignalR.Client;
using ShogiServer.WebApi.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PopulateScrollView : MonoBehaviour
{
    [SerializeField] private Transform m_ContentContainer;
    [SerializeField] private GameObject m_ItemPrefab;
    public TMP_InputField inputField;
    public GameObject invitePopup;
    public GameObject pendingInvitePopup;
    public TextMeshProUGUI nickDisplay;
    public Invitation invitation;
    private HubConnection connection1 = null!;
    private bool update = false;
    private bool startGame = false;
    private bool popupActive = false;
    List<ServerPlayer> list;
    string filter = "";
    public void UpdateView(List<ServerPlayer> l)
    {
        l = l.Where(x => x.Nickname.Contains(filter)).ToList();
        l = l.Where(a=>a.Id!=PlayerPasser.instance.hostPlayer.Id).ToList();
        var children = new List<GameObject>();
        foreach (Transform child in m_ContentContainer.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        foreach (ServerPlayer player in l)
        {
            var item_go = Instantiate(m_ItemPrefab);
            // do something with the instantiated item -- for instance
            item_go.GetComponentInChildren<Text>().text = player.Nickname;
            //parent the item to the content container
            item_go.transform.SetParent(m_ContentContainer);
            //reset the item's scale -- this can get munged with UI prefabs
            item_go.transform.localScale = Vector2.one;
            item_go.GetComponent<PlayerItemScript>().player = player;
        }
    }

    public async Task<HubConnection>  InitializeConnection() {

        connection1 = new HubConnectionBuilder()
                 .WithUrl(
                     "https://localhost:7080/shogi-hub").WithKeepAliveInterval(new TimeSpan(10)).WithAutomaticReconnect()
                 .Build();
        connection1.On<ServerPlayer>("SendPlayer", response => {
            PlayerPasser.instance.hostPlayer = response;
        });
        connection1.On<List<ServerPlayer>>("SendLobby", response => {
            list = response;
            update = true;
        });

        connection1.On<Invitation>("SendInvitation", response =>
        {
            invitation = response;
            popupActive = true;
        });
        connection1.On<GameDTO>("SendCreatedGame", response =>
        {
            PlayerPasser.instance.game = response;
            startGame = true;
        });
        PlayerPasser.instance.connection1 = connection1;
        await connection1.StartAsync();
        string s = "player" + new System.Random().Next(100000);
        await connection1.InvokeAsync("JoinLobby", s);
       
        return connection1;
    }

    public void Accept()
    {
        connection1.InvokeAsync("AcceptInvitation", new AcceptInvitationRequest(invitation.Id, PlayerPasser.instance.hostPlayer.Token));
        popupActive = false;
       
    }
    public void Cancel()
    {
        connection1.InvokeAsync("CancelInvitation", new CancelInvitationRequest(invitation.Id, PlayerPasser.instance.hostPlayer.Token));
    }
    public void Decline()
    {
        connection1.InvokeAsync("DeclineInvitation", new RejectInvitationRequest(invitation.Id, PlayerPasser.instance.hostPlayer.Token));
        popupActive = false;
    }
    public void FilterInput(string s)
    {
        filter = s;
        UpdateView(list);
    }

    void Start()
    {
        inputField.onEndEdit.AddListener(FilterInput);
        Task.Run(()=>InitializeConnection());
    }

    void Update()
    {
        if(popupActive)
        {
            invitePopup.SetActive(true);
        }
        else
        {
            invitePopup.SetActive(false);
        }
        if (update)
        {
            if(nickDisplay.text=="Nick: ")
                nickDisplay.text += (PlayerPasser.instance.hostPlayer.Nickname);
            UpdateView(list);
            update = false;
        }
        if (startGame)
        {
            ServerPlayer white = PlayerPasser.instance.game.WhitePlayer;
            ServerPlayer black = PlayerPasser.instance.game.BlackPlayer;
            PlayerPasser.instance.player1 = white.Id == PlayerPasser.instance.hostPlayer.Id ? new HumanPlayer(white.Nickname, true) { is_online = true} :new RemotePlayer(white.Nickname,true);
            PlayerPasser.instance.player2 = black.Id == PlayerPasser.instance.hostPlayer.Id ? new HumanPlayer(black.Nickname, false) { is_online = true } : new RemotePlayer(black.Nickname, false);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Sandbox");
        }
    }
}
