using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public Board board;
    private GameObject[,] pieces;

    public Player currentPlayer;
    public Player otherPlayer;

    public Player player1;
    public Player player2;

    public GameObject King;
    void Awake()
    {
        instance = this;
        }
    // Start is called before the first frame update
    void Start()
    {
        pieces = new GameObject[9, 9];

        player1 = new Player("player1", true);
        player2 = new Player("player2", false);

        currentPlayer = player1;
        otherPlayer = player2;
        if (NetworkManager.Singleton.IsServer) {
        AddPiece(King, player1,1,1);
        AddPiece(King, player2, 6, 6); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddPiece(GameObject prefab, Player player, int col, int row)
    {
        GameObject pieceObject = board.AddPiece(prefab, col, row);
        pieceObject.GetComponent<NetworkObject>().Spawn();
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
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
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
        Destroy(pieceToCapture);
    }
    

    public void ConvertFromUSI(string FENstring)
    {
        board = new Board();
        pieces = new GameObject[9, 9];

       var parseds = FENstring.Split(' ');
        
    }

}
