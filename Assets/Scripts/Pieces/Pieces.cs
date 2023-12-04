using System.Collections.Generic;
using UnityEngine;

public enum PieceType { King,Lance,Gold,Bishop,Knight,Pawn,Silver,Rook };

public abstract class Piece : MonoBehaviour
{
    public  PieceType type;

    public abstract List<Vector2Int> MoveLocations(Vector2Int gridPoint);
}
