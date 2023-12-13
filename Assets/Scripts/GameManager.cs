using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniJSON;
using Unity.Netcode;
using UnityEngine;
using ShogiEngineDllTests;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
        }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        pieces = new GameObject[9, 9];

        player1 = new HumanPlayer("player1", true);
        player2 = new HumanPlayer("player2", false);

        currentPlayer = player1;
        otherPlayer = player2;
        var fen = "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL b -";
        moves =  ShogiEngineInterface.GetAllMoves(fen);

        moves = moves.Concat(ShogiEngineInterface.GetAllMoves("lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL w -")).ToList();
        ConvertFromUSI(fen);
        rotateCamera = player1 is HumanPlayer && player2 is HumanPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        
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
    public void NextPlayer() 
    {
        moves = ShogiEngineInterface.GetAllMoves(convert2FEN());
        Player tempPlayer = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = tempPlayer;
        var o = ShogiEngineInterface.GetAllMoves(convert2FEN());
        moves = moves.Concat( ShogiEngineInterface.GetAllMoves(convert2FEN())).ToList();
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
        return currentPlayer.pieces.Contains(piece);
    }
    public void Move(GameObject piece, Vector2Int gridPoint)
    {
        Vector2Int startGridPoint = GridForPiece(piece);
        pieces[startGridPoint.x, startGridPoint.y] = null;
        pieces[gridPoint.x, gridPoint.y] = piece;
        board.MovePiece(piece, gridPoint);
        Promote(piece,gridPoint);
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
        hand.Remove(pieceType);
        AddPiece(d[char.ToLower(pieceType)], currentPlayer, gridPoint.x, gridPoint.y);
    }
    public void ConvertFromUSI(string FENstring)
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
                    AddPiece(d[char.ToLower(rows[i][j])], char.IsLower(rows[i][j]) ? player1 : player2, a, i,promoted);
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
    }
    public string convert2FEN()
    {
        Dictionary<Char, GameObject> d = new Dictionary<char, GameObject>() {

            {'k', King },
            {'r', Rook },
            {'b', Bishop },
            {'g', Gold},
            {'s', Silver },
            {'l', Lance },
            {'n', Knight },
            {'p', Pawn },
            
         
            
           
            
        };
        string fen = "";
        for(int i = 0; i < 9; i++)
        {
            int a = 0;
            for(int j = 0;j < 9; j++)
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

}
