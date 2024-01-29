using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniJSON;
using Unity.Netcode;
using UnityEngine;
using ShogiEngineDllTests;
using Microsoft.AspNetCore.SignalR.Client;
using ShogiServer.WebApi.Model;
using System.Text;
using System.Threading;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool update = false;
    public bool updateMove = false;
    public bool win = false;
    string move = "";
    public string fenBoard = "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL b -";
    public Board board;
    private GameObject[,] pieces;
    private bool rotateCamera = false;
    public List<char> hand = new List<char>();
    public Player currentPlayer;
    public Player otherPlayer;
    [SerializeField]
    private Camera mainCamera = default;
    public Player player1;
    public Player player2;
    public List<(Vector2Int, Vector2Int)> moves;
    public GameObject King;
    public GameObject Knight;
    public GameObject Rook;
    public GameObject Bishop;
    public GameObject Pawn;
    public GameObject Silver;
    public GameObject Gold;
    public GameObject Lance;
    void Awake()
    {

        instance = this;
        ShogiEngineInterface.CleanUp();
        bool init = ShogiEngineInterface.Init();
       
            if (PlayerPasser.instance.player1 is HumanPlayer &&PlayerPasser.instance.player2 is not HumanPlayer &&!bool.Parse(PlayerPasser.instance.configuration.GetValueOrDefault("StaticCamera")))
                mainCamera.GetComponent<CameraRotator>().rotate(180); 
        if(PlayerPasser.instance.connection1 != null) {
        PlayerPasser.instance.connection1.On<GameDTO>("SendGameState", response =>
        {
            GameManager.instance.fenBoard = response.BoardState;
            GameManager.instance.update = true;
            
        });}
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        pieces = new GameObject[9, 9];

        player1 = PlayerPasser.instance.player1;
        player2 = PlayerPasser.instance.player2; 

        currentPlayer = player2;
        otherPlayer = player1;
        BoardFromFen(fenBoard);
        rotateCamera = player1 is HumanPlayer && player2 is HumanPlayer;
        StartMakeMove();
    }

    public void BoardFromFen(string fen)
    {
        moves = ShogiEngineInterface.GetAllMoves(fen);
        int i = fen.IndexOf(' ');
        StringBuilder fen2 = new StringBuilder(fen);
        fen2[i + 1] = fen2[i + 1] == 'w' ? 'b' : 'w';
        
        moves = moves.Concat(ShogiEngineInterface.GetAllMoves(fen2.ToString())).ToList();
        ConvertFromUSI(fen);
    }

    // Update is called once per frame
    void Update()
    {
        if (win)
        {
            Win();
        }
        if (update)
        {
            update = false;
            BoardFromFen(fenBoard);
            
        }
        if(updateMove)
        {
            updateMove = false;
            Move2Board(move);
            
        }
        
    }
    public void AddPiece(GameObject prefab, Player player, int col, int row,bool promoted = false)
    {
        GameObject pieceObject = board.AddPiece(prefab, col, row);
        if(player.forward==-1)
        {
            pieceObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        pieceObject.GetComponent<Piece>().player = player;
        if (promoted)
            pieceObject.GetComponent<Piece>().Promote(pieceObject);
        player.pieces.Add(pieceObject);
        pieces[col, row] = pieceObject;
    }


    public void SelectPieceAtGrid(Vector2Int gridPoint)
    {
        GameObject selectedPiece = pieces[gridPoint.x, gridPoint.y];
        if (selectedPiece)
        {
            board.SelectPiece(selectedPiece);
        }
    }
    public Vector2Int GridForPiece(GameObject piece)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (pieces[i, j] == piece)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }
    public bool FriendlyPieceAt(Vector2Int gridPoint)
    {
        GameObject piece = PieceAtGrid(gridPoint);

        if (piece == null)
        {
            return false;
        }

        if (otherPlayer.pieces.Contains(piece))
        {
            return false;
        }

        return true;
    }
    public GameObject PieceAtGrid(Vector2Int gridPoint)
    {
        if (gridPoint.x > 8 || gridPoint.y > 8 || gridPoint.x < 0 || gridPoint.y < 0)
        {
            return null;
        }
        return pieces[gridPoint.x, gridPoint.y];
    }
    public List<Vector2Int> MovesForPiece(GameObject pieceObject)
    {
        if(pieceObject == null) return new List<Vector2Int>();
        Piece piece = pieceObject.GetComponent<Piece>();
        Vector2Int gridPoint = GridForPiece(pieceObject);
        List<Vector2Int> locations = piece.MoveLocations(gridPoint);

        // filter out offboard locations
        locations.RemoveAll(gp => gp.x < 0 || gp.x > 8 || gp.y < 0 || gp.y > 8);

        // filter out locations with friendly piece
        locations.RemoveAll(gp => FriendlyPieceAt(gp));

        return locations;
    }

    public void StartMakeMove()
    { 
        
        string fen = convert2FEN();
        UnityEngine.Debug.Log("current board " + fen);
        if (ShogiEngineInterface.GetAllMoves(fen).Count == 0)
            Win();
        new Thread(() => { 
            move = currentPlayer.MakeMove(fen);
            updateMove = true;
        }).Start();

    }
    public void NextPlayer() 
    {
        moves = ShogiEngineInterface.GetAllMoves(convert2FEN());
        Player tempPlayer = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = tempPlayer;
        var o = ShogiEngineInterface.GetAllMoves(convert2FEN());
        moves = moves.Concat(o).ToList();
        StartMakeMove();
    }
    public void SelectPiece(GameObject piece)
    {
        board.SelectPiece(piece);
    }

    public void DeselectPiece(GameObject piece)
    {
        board.DeselectPiece(piece);
    }

    public bool DoesPieceBelongToCurrentPlayer(GameObject piece)
    {
        return currentPlayer.pieces.Contains(piece) && currentPlayer is HumanPlayer;
    }
    public void Move(GameObject piece, Vector2Int gridPoint)
    {
        Vector2Int startGridPoint = GridForPiece(piece);
        pieces[startGridPoint.x, startGridPoint.y] = null;
        pieces[gridPoint.x, gridPoint.y] = piece;
        board.MovePiece(piece, gridPoint);
        Promote(piece,gridPoint);
    }
    public void MovePiece(GameObject piece, Vector2Int gridPoint)
    {
        if (GameManager.instance.PieceAtGrid(gridPoint) == null)
        {
            GameManager.instance.Move(piece, gridPoint);
        }
        else
        {
            GameManager.instance.CapturePieceAt(gridPoint);
            GameManager.instance.Move(piece, gridPoint);
        }
    }
    public void CapturePieceAt(Vector2Int gridPoint)
    {
        GameObject pieceToCapture = PieceAtGrid(gridPoint);
        currentPlayer.capturedPieces.Add(pieceToCapture);
        pieces[gridPoint.x, gridPoint.y] = null;
        SendToHand(pieceToCapture);
        board.RemovePiece(pieceToCapture);
    }

    public void Promote(GameObject piece, Vector2Int gridPoint)
    {
        Piece p = piece.GetComponent<Piece>();
        if (currentPlayer == player1 && gridPoint.y >= 6)
            p.Promote(piece);
        else if (currentPlayer == player2 && gridPoint.y < 3)
            p.Promote(piece);
    }
    private void Win()
    {
        PlayerPasser.instance.Winner = currentPlayer;
        ShogiEngineInterface.CleanUp();
        UnityEngine.SceneManagement.SceneManager.LoadScene("WinScreen");
    }
    public void SendToHand(GameObject piece)
    {
        
        char c = GameObject2Char(piece);
        if(c=='k')
            UnityEngine.SceneManagement.SceneManager.LoadScene("WinScreen");
        if (currentPlayer == player2) c = char.ToUpper(c);
        hand.Add(c);
    }

    public char GameObject2Char(GameObject g)
    {
        Dictionary<Char, GameObject> d = new Dictionary<char, GameObject>() {

            {'k', King },
            {'r', Rook },
            {'p', Pawn },
            {'l', Lance },
            {'n', Knight },
            {'s', Silver },
            {'g', Gold},
            {'b', Bishop }
        };

        return d.FirstOrDefault(x => x.Value.GetComponent<Piece>().type == g.GetComponent<Piece>().type).Key;
    }

    public GameObject Char2GameObject(char c)
    {
        Dictionary<Char, GameObject> d = new Dictionary<char, GameObject>() {

            {'k', King },
            {'r', Rook },
            {'p', Pawn },
            {'l', Lance },
            {'n', Knight },
            {'s', Silver },
            {'g', Gold},
            {'b', Bishop }
        };
        return d[c];
    }

    public void DropPiece(char pieceType,Vector2Int gridPoint)
    {
        Dictionary<Char, GameObject> d = new Dictionary<char, GameObject>() {

            {'k', King },
            {'r', Rook },
            {'p', Pawn },
            {'l', Lance },
            {'n', Knight },
            {'s', Silver },
            {'g', Gold},
            {'b', Bishop }
        };
        string o = "";
        foreach (var e in hand)
            o+=(e);
        Debug.Log(pieceType);
        Debug.Log("pre " + o);
        hand.Remove(pieceType);
        AddPiece(d[char.ToLower(pieceType)], currentPlayer, gridPoint.x, gridPoint.y);
         o = "";
        foreach (var e in hand)
            o+=(e);
        Debug.Log("post " + o);
    }
    public void LogBoard()
    {

        Dictionary<Char, GameObject> d = new Dictionary<char, GameObject>() {

            {'k', King },
            {'r', Rook },
            {'p', Pawn },
            {'l', Lance },
            {'n', Knight },
            {'s', Silver },
            {'g', Gold},
            {'b', Bishop }
        };
        string s = "Board:\n";
        for(int i = 0; i < 9; i++) { 
            for(int j = 0; j < 9; j++)
            {
                if (pieces[i, j]== null)
                {
                    s += " 0 ";
                    continue;
                }
                var x = d.FirstOrDefault(x => x.Value.GetComponent<Piece>().type == pieces[i,j].GetComponent<Piece>().type);
                var z = x.Key;
                s += $" {z} ";
            }
            s += "\n";
        }
        UnityEngine.Debug.Log(s);
    }

    public void ConvertFromUSI(string FENstring)
    {
        foreach(var e in pieces)
            if(e!=null) Destroy(e);

        Dictionary<Char, GameObject> d = new Dictionary<char, GameObject>() {

            {'k', King },
            {'r', Rook },
            {'p', Pawn },
            {'l', Lance },
            {'n', Knight },
            {'s', Silver },
            {'g', Gold},
            {'b', Bishop }
        };
        pieces = new GameObject[9, 9];

        var parseds = FENstring.Split(' ');
        var rows = parseds[0].Split('/');
        bool promoted = false;
        for(int i = 0; i < 9; i++)
        {
            int a = 0;
            for(int j = 0; j < rows[i].Length;j++)
            {
                if (int.TryParse(rows[i][j].ToString(),out int b))
                {
                    a += b;
                }
                else if (rows[i][j] == '+')
                {
                    promoted = true;
                }
                else
                {
                    var s = d[char.ToLower(rows[i][j])];
                    var x = char.ToLower(rows[i][j]);
                    AddPiece(d[char.ToLower(rows[i][j])], char.IsLower(rows[i][j]) ? player1 : player2, 8 - a, i,promoted);
                     promoted = false;
                    a++;
                }
            }
        }
        string parsedhand = "";
        int n = 1;
        int t;
        foreach(var e in parseds[2])
        {
            if (!int.TryParse(e.ToString(),out t))
            {
                for (int i = 0; i < n; i++)
                {
                    parsedhand += e;
                }
                n = 1;
            }
            else
            {
                n = t;
            }
        }
        hand = parsedhand.ToList();
        currentPlayer = parseds[1] == "w"? player1 : player2;
        otherPlayer = parseds[1] != "w" ? player1 : player2;
        LogBoard();
    }
    public string convert2FEN()
    {
        Dictionary<Char, GameObject> d = new Dictionary<char, GameObject>() {

            {'k', King },
            {'r', Rook },
            {'b', Bishop },
            {'g', Gold},
            {'s', Silver },
            {'n', Knight },
            {'l', Lance },
            {'p', Pawn },
        };
        string fen = "";
        for(int i = 0; i < 9; i++)
        {
            int a = 0;
            for(int j = 8;j >=0; j--)
            {
                if (pieces[j, i] == null) { 
                    a++;
                    continue;
                }
                var x = d.FirstOrDefault(x => x.Value.GetComponent<Piece>().type == pieces[j, i].GetComponent<Piece>().type);
                var z = x.Key;
                
                if(a!=0)
                    fen += a;
                a = 0;
                fen += pieces[j, i].GetComponent<Piece>().promoted ? "+" : "";
                fen += (pieces[j, i].GetComponent<Piece>().player)==player1? char.ToLower(z):char.ToUpper(z);
            }
            if (a != 0)
                fen += a;
            fen += "/";
        }
        fen = fen.Remove(fen.Length - 1);

        fen += " ";
        fen += currentPlayer == player1 ? "w" : "b";
        fen += " ";
        if (hand.Count() == 0)
            fen += "-";
        foreach (var piecetype in d)
        {
            char type = char.ToUpper(piecetype.Key);
            int handcount = hand.Where(x => x == type).Count();
            if (handcount > 0)
            {
                if (handcount > 1)
                {
                    fen += handcount.ToString();
                }
                fen += type;
            }
        }

        foreach(var piecetype in d)
        {
            int handcount = hand.Where(x => x == piecetype.Key).Count();
            if (handcount > 0)
            {
                if (handcount > 1)
                {
                    fen += handcount.ToString();
                }
                fen += piecetype.Key;
            }
        }
       
        return fen;
    }


    public void Move2Board(string move)
    {
        if(move.Length == 0) return;
        if(move.Contains("+")) {
            var m = ShogiEngineInterface.Move2Coord(move);
            
            MovePiece(pieces[m.Item1.x, m.Item1.y], m.Item2);
            Promote(pieces[m.Item2.x, m.Item2.y], m.Item2);

        }
        else if(move.Any(char.IsUpper))
        {
            var m = ShogiEngineInterface.Move2Coord(move);
            UnityEngine.Debug.Log($"drop {move} {m.Item2.x} {m.Item2.y}");
            var p= move.Where(x => char.IsUpper(x)).First();
            GameManager.instance.DropPiece(currentPlayer==player1?char.ToLower(p):char.ToUpper(p), m.Item2);
        }
        else
        {
           var m = ShogiEngineInterface.Move2Coord(move);
           MovePiece(pieces[m.Item1.x, m.Item1.y], m.Item2);
        }
        NextPlayer();
    }
}
