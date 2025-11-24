using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyDummy : CharacterBase
{
    public int hp = 3;
    public int damage = 1;

    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;
    public Transform player;
    public float moveSpeed = 2f;

    private Animator animator;
    private Vector3Int lastMoveTarget;
    private PlayerGridMovement playerScript;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (player != null)
            playerScript = player.GetComponent<PlayerGridMovement>();
    }

    void Update()
    {
        if (player != null)
            LookAtPlayer();
    }

    void LookAtPlayer()
    {
        if (player.position.x > transform.position.x)
            transform.localScale = Vector3.one;
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    public override IEnumerator TakeTurn()
    {
        TurnDone = false;
        yield return EnemyTurnRoutine();
        TurnDone = true;
    }

    IEnumerator EnemyTurnRoutine()
    {
        if (player == null) yield break;

        Vector3Int enemyPos = groundTilemap.WorldToCell(transform.position);
        Vector3Int playerPos = groundTilemap.WorldToCell(player.position);

        int dx = playerPos.x - enemyPos.x;
        int dy = playerPos.y - enemyPos.y;

        if (Mathf.Abs(dx) + Mathf.Abs(dy) == 1)
        {
            yield return AttackRoutine();
            yield break;
        }

        Vector3Int moveTo = enemyPos;

        if (Mathf.Abs(dx) > Mathf.Abs(dy))
            moveTo.x += dx > 0 ? 1 : -1;
        else
            moveTo.y += dy > 0 ? 1 : -1;

        if (groundTilemap.HasTile(moveTo) && !obstacleTilemap.HasTile(moveTo))
        {
            lastMoveTarget = moveTo;
            yield return MoveRoutine();
        }
    }

    IEnumerator AttackRoutine()
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.4f);
        playerScript?.TakeDamage(damage);
    }

    IEnumerator MoveRoutine()
    {
        Vector3 target = groundTilemap.GetCellCenterWorld(lastMoveTarget);
        animator.SetBool("IsMoving", true);

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        animator.SetBool("IsMoving", false);
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            IsDead = true;
            Destroy(gameObject);
        }
    }
}