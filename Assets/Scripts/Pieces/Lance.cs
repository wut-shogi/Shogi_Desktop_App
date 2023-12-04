using System.Collections.Generic;
using UnityEngine;

class Lance : Piece
{
    Lance() { }
    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
    {
        List<Vector2Int> locations = new List<Vector2Int>();
        for(int i = 0;i<8;i++)
            for(int j = 0; j < 8; j++)
            {
                locations.Add(new Vector2Int(i,j));
            }

        return locations;
    }
}