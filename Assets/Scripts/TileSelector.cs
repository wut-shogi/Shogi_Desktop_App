
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    public static TileSelector instance;
    public GameObject tileHighlightPrefab;
    private List<Vector2Int> moveLocations;
    private List<GameObject> locationHighlights;
    private GameObject tileHighlight;
    public GameObject moveLocationPrefab;
    public GameObject attackLocationPrefab;
    private Vector2Int current;
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
        if(pause) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 point = hit.point;
            Vector2Int gridPoint = Geometry.GridFromPoint(point);

            tileHighlight.SetActive(true);
            if(gridPoint.x<0|| gridPoint.x>8|| gridPoint.y<0|| gridPoint.y>8) {
                tileHighlight.SetActive(false);
            }
            tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);
            if (Input.GetMouseButtonDown(0))
            {
                GameObject selectedPiece = GameManager.instance.PieceAtGrid(gridPoint);
                if (GameManager.instance.DoesPieceBelongToCurrentPlayer(selectedPiece))
                {
                    GameManager.instance.SelectPiece(selectedPiece);
                    // Reference Point 1: add ExitState call here later
                    ExitState(selectedPiece);
                }
            }
            else
            {
                GameObject selectedPiece = GameManager.instance.PieceAtGrid(gridPoint);

                if(gridPoint != current)
                {
                    foreach (GameObject highlight in locationHighlights)
                    {
                        Destroy(highlight);
                    }
                moveLocations = GameManager.instance.MovesForPiece(selectedPiece);
                locationHighlights = new List<GameObject>();

                foreach (Vector2Int loc in moveLocations)
                {
                    GameObject highlight;
                    if (GameManager.instance.PieceAtGrid(loc))
                    {
                        highlight = Instantiate(attackLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
                    }
                    else
                    {
                        highlight = Instantiate(moveLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
                    }
                    locationHighlights.Add(highlight);
                }
                }
            }
        }
        else
        {
            tileHighlight.SetActive(false);
        }
    }

    public void EnterState()
    {
        enabled = true;
    }

    private void ExitState(GameObject movingPiece)
    {
        this.enabled = false;
        tileHighlight.SetActive(false);
        MoveSelector move = GetComponent<MoveSelector>();
        move.EnterState(movingPiece);
    }
    public void Disable()
    {
        this.enabled = false;
        tileHighlight.SetActive(false);
    }
}
