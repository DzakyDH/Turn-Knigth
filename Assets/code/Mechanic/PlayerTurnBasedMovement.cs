using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerGridMovement : MonoBehaviour
{
    public Tilemap groundTilemap;    // Tilemap tempat player boleh berjalan
    public Tilemap obstacleTilemap;  // Tilemap berisi obstacle/penghalang
    public float moveSpeed = 5f;
    public LayerMask enemyLayer;
    public EnemyDummy enemytarget;

    private bool isSelected = false;
    private bool isMoving = false;
    private Vector3 targetPosition;

    private Animator animator;
    private GameObject outline;

    void Start()
    {
        SnapPlayerToTile();
        animator = GetComponent<Animator>();

        outline = transform.Find("outline")?.gameObject;
        if (outline != null) outline.SetActive(false);
    }

    void Update()
    {
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            MoveToTarget();
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            HandlePlayerClick();
        }


    }

    void SnapPlayerToTile()
    {
        Vector3Int gridPos = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(gridPos);
        targetPosition = transform.position;
    }

    void HandlePlayerClick()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));

        Vector3Int clickedGridPos = groundTilemap.WorldToCell(mouseWorldPos);
        Vector3Int playerGridPos = groundTilemap.WorldToCell(transform.position);

        if (!isSelected)
        {
            if(clickedGridPos == playerGridPos)
            {
                isSelected = true;
                if (outline != null) outline.SetActive(true);
                return;
            }
            return;
        }
        outline?.SetActive(false);
        isSelected = false;

        int dx = Mathf.Abs(clickedGridPos.x - playerGridPos.x);
        int dy = Mathf.Abs(clickedGridPos.y - playerGridPos.y);

        if (dx + dy != 1) 
            return;

        Collider2D enemy = Physics2D.OverlapCircle(groundTilemap.GetCellCenterWorld
            (clickedGridPos),0.2f , enemyLayer);
  
        if (enemy != null)
        {
            int dist = Mathf.Abs(clickedGridPos.x - playerGridPos.x)
                + Mathf.Abs(clickedGridPos.y - playerGridPos.y);

            if (dist == 1)
            {
                Attack(enemy.GetComponent<EnemyDummy>());
            }
            else
            {
                Debug.Log("Musuh terlali jauh!");
            }
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
    void Attack(EnemyDummy enemy)
    {
        enemytarget = enemy;
        animator.SetTrigger("Attack");
    }
    public void DealDamage()
    {
        if (enemytarget != null)
            enemytarget.TakeDamage(1);
    }
}