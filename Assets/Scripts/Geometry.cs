using Unity.VisualScripting;
using UnityEngine;

public class Geometry
{
    static public float tileSize = Mathf.Abs((-1f - 1f) / 9f);
    static public float leftCornerOffset = -1f + tileSize/2f;
    static public Vector3 PointFromGrid(Vector2Int gridPoint)
    {
        float x = leftCornerOffset + tileSize * gridPoint.x;
        float z = leftCornerOffset + tileSize * gridPoint.y;
        return new Vector3(x, 0.06f, z);
    }

    static public Vector2Int GridPoint(int col, int row)
    {
        return new Vector2Int(col, row);
    }

    static public Vector2Int GridFromPoint(Vector3 point)
    {
        int col = Mathf.RoundToInt((-leftCornerOffset + point.x) / tileSize);
        int row = Mathf.RoundToInt((-leftCornerOffset + point.z) / tileSize);
        return new Vector2Int(col, row);
    }
}
