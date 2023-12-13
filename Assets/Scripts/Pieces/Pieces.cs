using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PieceType { King,Lance,Gold,Bishop,Knight,Pawn,Silver,Rook };

public abstract class Piece : MonoBehaviour
{
    public  PieceType type;
    public bool promoted = false;
    public Player player;
    public List<Vector2Int> MoveLocations(Vector2Int gridPoint) { 
        var s = GameManager.instance.moves.Where(x => x.Item1 == gridPoint).Select(x => x.Item2).ToList();
        return GameManager.instance.moves.Where(x=>x.Item1== gridPoint).Select(x=>x.Item2).ToList();
    
    }

    public void Promote(GameObject piece)
    {
        if (promoted) 
            return;
        promoted = true;
        piece.transform.Rotate(new Vector3(0, 0, 180));
    }
}
