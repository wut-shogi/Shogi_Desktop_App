using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniJSON;
using Unity.Netcode;
using UnityEngine;

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
        ConvertFromUSI("lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL b - 1");
        rotateCamera = player1 is HumanPlayer && player2 is HumanPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddPiece(GameObject prefab, Player player, int col, int row)
    {
        GameObject pieceObject = board.AddPiece(prefab, col, row);
        if(player.forward==-1)
        {
            pieceObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
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
        Player tempPlayer = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = tempPlayer;
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
    }
    public void CapturePieceAt(Vector2Int gridPoint)
    {
        GameObject pieceToCapture = PieceAtGrid(gridPoint);
        currentPlayer.capturedPieces.Add(pieceToCapture);
        pieces[gridPoint.x, gridPoint.y] = null;
        SendToHand(pieceToCapture);
        board.RemovePiece(pieceToCapture);
    }
    public void SendToHand(GameObject piece)
    {
        
        char c = GameObject2Char(piece);
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

        for(int i = 0; i < 9; i++)
        {
            int a = 0;
            for(int j = 0; j < rows[i].Length;j++)
            {
                if (int.TryParse(rows[i][j].ToString(),out int b))
                {
                    a += b;
                }
                else
                {
                    var s = d[char.ToLower(rows[i][j])];
                    var x = char.ToLower(rows[i][j]);
                    AddPiece(d[char.ToLower(rows[i][j])], char.IsLower(rows[i][j]) ? player1 : player2, a, i);
                    a++;
                }
            }
        }
        currentPlayer = parseds[1] != "w"?player1 : player2;
        otherPlayer = parseds[1] == "w" ? player1 : player2;
    }

}
