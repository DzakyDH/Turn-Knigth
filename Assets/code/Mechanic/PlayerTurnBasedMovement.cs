using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerGridMovement : MonoBehaviour
{
    public Tilemap groundTilemap;    // Tilemap tempat player boleh berjalan
    public Tilemap obstacleTilemap;  // Tilemap berisi obstacle/penghalang
    public float moveSpeed = 5f;

    private bool isMoving = false;
    private Vector3 targetPosition;

    private Animator animator;

    void Start()
    {
        // Menempatkan player tepat di tengah grid pada awalnya
        SnapPlayerToTile();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Menyesuaikan animasi berdasarkan gerakan
        animator.SetBool("IsMoving", isMoving);

        if (!isMoving)
        {
            HandleMouseClick();
        }
        else
        {
            MoveToTarget();
        }
    }

    void SnapPlayerToTile()
    {
        Vector3Int gridPos = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(gridPos);
        targetPosition = transform.position;
    }

    void HandleMouseClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f)
        );

        Vector3Int clickedGridPos = groundTilemap.WorldToCell(mouseWorldPos);
        Vector3Int playerGridPos = groundTilemap.WorldToCell(transform.position);

        int dx = Mathf.Abs(clickedGridPos.x - playerGridPos.x);
        int dy = Mathf.Abs(clickedGridPos.y - playerGridPos.y);

        // Hanya boleh bergerak 1 tile dan tidak diagonal
        if (dx + dy != 1)
        {
            Debug.Log("Klik tile terlalu jauh atau diagonal.");
            return;
        }

        // Pengecekan apakah tile adalah ground dan bukan obstacle
        if (groundTilemap.HasTile(clickedGridPos) && !obstacleTilemap.HasTile(clickedGridPos))
        {
            // Flip sprite berdasarkan arah
            if (clickedGridPos.x > playerGridPos.x)
                transform.localScale = new Vector3(1, 1, 1); // facing right
            else if (clickedGridPos.x < playerGridPos.x)
                transform.localScale = new Vector3(-1, 1, 1); // facing left

            targetPosition = groundTilemap.GetCellCenterWorld(clickedGridPos);
            isMoving = true;
        }
        else
        {
            Debug.Log("Tile tersebut bukan ground atau ada obstacle-nya.");
        }
    }

    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }
}