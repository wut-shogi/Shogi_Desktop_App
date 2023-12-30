using System.Collections.Generic;
using UnityEngine;

public abstract class Player
{
    public List<GameObject> pieces;
    public List<GameObject> capturedPieces;

    public string name;
    public int forward;
    public abstract string MakeMove(string fen);
    public Player(string name, bool positiveZMovement)
    {
        this.name = name;
        pieces = new List<GameObject>();
        capturedPieces = new List<GameObject>();

        if (positiveZMovement == true)
        {
            this.forward = 1;
        }
        else
        {
            this.forward = -1;
        }
    }
}
