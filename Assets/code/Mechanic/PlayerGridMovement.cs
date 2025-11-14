using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class PlayerGridMovement : MonoBehaviour
{
    public Tilemap groundTilemap;

    public GameObject footIconPrefab;
    public GameObject pathDotPrefab;

    private Vector3Int selectedTile;
    private bool hasSelected = false;

    private GameObject footIcon;
    private List<GameObject> pathDoths = new List<GameObject>();

    private Vector3Int currentfile;

    private void Start()
    {
        currentfile = groundTilemap.WorldToCell(transform.position);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int clickedTile = groundTilemap.WorldToCell(world);

            if (!groundTilemap.HasTile(clickedTile))
                return;

            if (hasSelected && clickedTile == selectedTile)
            {
                MoveAlongPath(clickedTile);
                ClearPathPreview();
                hasSelected = false;
                return;
            }
            selectedTile = clickedTile;
            hasSelected = true;

            PreviewPath(clickedTile);
        }
    }
    void PreviewPath(Vector3Int targetTile)
    {
        ClearPathPreview();

        footIcon = Instantiate(footIconPrefab, groundTilemap.GetCellCenterLocal(targetTile),Quaternion.identity);
        List<Vector3Int> path = GeneratesSimplePath(currentfile, targetTile);

        foreach(var cell in path)
        {
            GameObject dot = Instantiate(pathDotPrefab, groundTilemap.GetCellCenterWorld(cell), Quaternion.identity);
            pathDoths.Add(dot);
        }
    }
    List<Vector3Int> GeneratesSimplePath(Vector3Int start, Vector3Int end)
    {
        List<Vector3Int>path = new List<Vector3Int>();
        Vector3Int temp = start;

        while (temp.x != end.x)
        {
            temp.x += temp.x < end.x ? 1 : -1;
            path.Add(temp);
        }
        while (temp.y != end.y)
        {
            temp.y += temp.y < end.y ? 1 : -1;
            path.Add(temp);
        }
        return path;
    }
    void ClearPathPreview()
    {
        if (footIcon != null) Destroy(footIcon);
        foreach (var dot in pathDoths) Destroy(dot);
        pathDoths.Clear();
    }
    void MoveAlongPath(Vector3Int targetTile)
    {
        List<Vector3Int> path = GeneratesSimplePath (currentfile, targetTile);
        StartCoroutine(MoveStepByStep(path));
        currentfile = targetTile;
    }
    System.Collections.IEnumerator MoveStepByStep(List<Vector3Int> path)
    {
        foreach (var step in path)
        {
            Vector3 pos = groundTilemap.GetCellCenterWorld(step);
            while (Vector3.Distance(transform.position, pos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * 3f);
                yield return null;
            }
        }
    }
}
