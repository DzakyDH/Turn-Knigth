using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class PlayerGridMovement : CharacterBase
{
    [Header("Tilemaps & Movement")]
    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;
    public float moveSpeed = 5f;

    [Header("Combat")]
    public LayerMask enemyLayer;
    public EnemyDummy enemyTarget;
    public int damage = 1;
    public int maxHP = 3;

    private bool isSelected = false;
    private bool isMoving = false;
    private Vector3 targetPosition;
    private int currentHP;

    private Animator animator;
    private GameObject outline;

    void Start()
    {
        SnapPlayerToTile();
        animator = GetComponent<Animator>();
        outline = transform.Find("outline")?.gameObject;
        if (outline != null) outline.SetActive(false);
        currentHP = maxHP;
    }

    void Update()
    {
        animator.SetBool("IsMoving", isMoving);

        if (!TurnDone && isMoving)
            MoveToTarget();

        else if (!TurnDone && Input.GetMouseButtonDown(0))
            HandlePlayerClick();
 
    }

    public override IEnumerator TakeTurn()
    {
        TurnDone = false;
        isSelected = false;
        outline?.SetActive(false);

        while (!TurnDone)
            yield return null;
    }

    void SnapPlayerToTile()
    {
        Vector3Int gridPos = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(gridPos);
        targetPosition = transform.position;
    }

    void HandlePlayerClick()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int clickedGridPos = groundTilemap.WorldToCell(mouseWorldPos);
        Vector3Int playerGridPos = groundTilemap.WorldToCell(transform.position);

        // Pilih player
        if (!isSelected && clickedGridPos == playerGridPos)
        {
            isSelected = true;
            outline?.SetActive(true);
            return;
        }

        outline?.SetActive(false);
        isSelected = false;

        int dx = Mathf.Abs(clickedGridPos.x - playerGridPos.x);
        int dy = Mathf.Abs(clickedGridPos.y - playerGridPos.y);
        if (dx + dy != 1) return;

        Collider2D enemyCol = Physics2D.OverlapCircle(
            groundTilemap.GetCellCenterWorld(clickedGridPos), 0.2f, enemyLayer);

        if (enemyCol != null)
        {
            enemyTarget = enemyCol.GetComponent<EnemyDummy>();
            StartCoroutine(AttackRoutine());
            return;
        }

        // Move ke tile kosong
        if (groundTilemap.HasTile(clickedGridPos) && !obstacleTilemap.HasTile(clickedGridPos))
        {
            targetPosition = groundTilemap.GetCellCenterWorld(clickedGridPos);
            transform.localScale = clickedGridPos.x > playerGridPos.x ? Vector3.one : new Vector3(-1, 1, 1);
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
            EndTurn();
        }
    }

    IEnumerator AttackRoutine()
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.4f);
        enemyTarget?.TakeDamage(damage);
        EndTurn();
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
        {
            IsDead = true;
            Destroy(gameObject);
        }
    }

    public override void EndTurn()
    {
        base.EndTurn();
        isMoving = false;
        enemyTarget = null;
    }
}