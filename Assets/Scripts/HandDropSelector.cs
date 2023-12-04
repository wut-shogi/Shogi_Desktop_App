using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDropSelector : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject tileHighlightPrefab;
    private List<Vector2Int> moveLocations = new List<Vector2Int>();

    public GameObject moveLocationPrefab;
    private List<GameObject> locationHighlights;
    private GameObject tileHighlight;
    private Vector2Int current;
    public static HandDropSelector instance;
    char pieceType;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        Vector2Int gridPoint = Geometry.GridPoint(0, 0);
        Vector3 point = Geometry.PointFromGrid(gridPoint);
        tileHighlight = Instantiate(tileHighlightPrefab, point, Quaternion.identity, gameObject.transform);
        locationHighlights = new List<GameObject>();

        tileHighlight.SetActive(false);
    }
    bool pause = false;
    void Update()
    {
        if (pause) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 point = hit.point;
            Vector2Int gridPoint = Geometry.GridFromPoint(point);

            tileHighlight.SetActive(true);
            if (gridPoint.x < 0 || gridPoint.x > 8 || gridPoint.y < 0 || gridPoint.y > 8)
            {
                tileHighlight.SetActive(false);
            }
            tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);
            if (Input.GetMouseButtonDown(0))
            {
                if (moveLocations.Contains(gridPoint))
                {
                    ExitState(gridPoint);
                }
            }
        }
        else
        {
            tileHighlight.SetActive(false);
        }
    }

    public void EnterState(char pieceType)
    {
        this.pieceType = pieceType;
        TileSelector.instance.Disable();
        enabled = true;
       for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                if(GameManager.instance.PieceAtGrid(new Vector2Int(i, j)) == null)
                {
                    moveLocations.Add(new Vector2Int(i, j));
                }
            }
        }
        foreach (Vector2Int loc in moveLocations)
        {
            GameObject highlight;
            highlight = Instantiate(moveLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
            locationHighlights.Add(highlight);
        }
    }

    private void ExitState(Vector2Int gridPoint)
    {
        this.enabled = false;
        tileHighlight.SetActive(false);
        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }
        moveLocations.Clear();
        GameManager.instance.DropPiece(pieceType, gridPoint);
        GameManager.instance.NextPlayer();
        TileSelector.instance.EnterState();
        
    }
}
