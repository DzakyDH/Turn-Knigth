using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class PlayerGridMovement : MonoBehaviour
{
    public Tilemap groundTilemap;   // <- ini WAJIB kamu isi di inspector
    public float moveSpeed = 4f;
    public SpriteRenderer spriteRenderer;

    private bool isMoving = false;

    private void Start()
    {
        Vector3Int cell = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(cell);
    }
    private void Update()
    {
        if (isMoving) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = groundTilemap.WorldToCell(world);

            if (!groundTilemap.HasTile(cell))
                return;

            TryMove(cell);
        }
    }
    void LateUpdate()
    {
        spriteRenderer.sortingOrder = (int)(-transform.position.y * 10);
    }

    void TryMove(Vector3Int targetCell)
    {
        Vector3 current = groundTilemap.WorldToCell(transform.position);

        // Hanya boleh gerak 1 langkah
        if (Mathf.Abs(targetCell.x - current.x) + Mathf.Abs(targetCell.y - current.y) != 1)
            return;

        // Convert cell ke world posisi tengah tile
        Vector3 targetPos = groundTilemap.GetCellCenterWorld(targetCell);

        StartCoroutine(MoveToTile(targetPos));
    }

    IEnumerator MoveToTile(Vector3 target)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }
}