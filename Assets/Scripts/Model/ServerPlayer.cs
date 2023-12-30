using System;

namespace ShogiServer.WebApi.Model
{

    public record InviteRequest
    {
        public string InvitedNickname { get; set; }
        public string Token { get; set; }
        public InviteRequest(string InvitedNickname, string Token)
        {
            this.InvitedNickname = InvitedNickname;
            this.Token = Token;
        }
    };
    public record RejectInvitationRequest
    {
        public Guid InvitationId { get; set; }
        public string Token { get; set; }
        public RejectInvitationRequest(Guid InvitationId, string Token)
        {
            this.InvitationId = InvitationId;
            this.Token = Token;
        }

    }
    public record CancelInvitationRequest
    {

        public Guid InvitationId { get; set; }
        public string Token { get; set; }
        public CancelInvitationRequest(Guid InvitationId, string Token)
        {
            this.InvitationId = InvitationId;
            this.Token = Token;
        }
    };
    public record AcceptInvitationRequest
    {
        public Guid InvitationId { get; set; }
        public string Token { get; set; }
        public AcceptInvitationRequest(Guid InvitationId, string Token)
        {
            this.InvitationId = InvitationId;
            this.Token = Token;
        }

    }
    public record MakeMoveRequest
    {
        public Guid GameId { get; set; }
        public string Token { get; set; }
        public string Move { get; set; }
        public MakeMoveRequest(Guid GameId, string Token, string Move)
        {
            this.GameId = GameId;
            this.Token = Token;
            this.Move = Move;
        }
    }

    public class ServerPlayer
    {
        public Guid Id { get; set; }

        public string Nickname { get; set; } = null!;

        public string ConnectionId { get; set; } = null!;

        public string Token { get; set; } = null!;

       
        public PlayerState State { get; set; } = PlayerState.Ready;

        public Invitation? SentInvitation { get; set; } = null!;

        public Invitation? ReceivedInvitation { get; set; } = null!;

        public GameDTO? GameAsBlack { get; set; } = null!;
        public GameDTO? GameAsWhite { get; set; } = null!;
    }

    public enum PlayerState
    {
        Ready, Inviting, Playing
    }
    public class GameDTO
    {
        public Guid Id { get; set; }
        public ServerPlayer BlackPlayer { get; set; } = null!;
        public ServerPlayer WhitePlayer { get; set; } = null!;
        public string BoardState { get; set; } = null!;
    }
    public class Invitation
    {
        public Guid Id { get; set; }

        public Guid InvitingPlayerId { get; set; }
        public ServerPlayer InvitingPlayer { get; set; } = null!;


        public Guid InvitedPlayerId { get; set; }
        public ServerPlayer InvitedPlayer { get; set; } = null!;
    }
}
