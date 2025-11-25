using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyDummy : MonoBehaviour
{
    public int hp = 3;
    public int damage = 1;

    public Tilemap groundTilemap;
    public Transform player;
    public float moveSpeed;

    private Animator animator;
    private Vector3Int LastMoveTarget;

    private PlayerGridMovement playerScript;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (player != null)
            playerScript = player.GetComponent<PlayerGridMovement>();
    }

    private void Update()
    {
        if (player != null)
            LookAtPlayer();
    }
    void LookAtPlayer()
    {

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else 
            transform.localScale = new Vector3(-1, 1, 1);
    }
    public void TakeDamage(int dmg)
    {
        hp -= dmg;    
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void EnemyTurn()
    {
        if (player == null) return;

        Vector3Int enemyPos = groundTilemap.WorldToCell(transform.position);
        Vector3Int playerpos = groundTilemap.WorldToCell(player.position);

        int dx = playerpos.x - enemyPos.x;
        int dy = playerpos.y - enemyPos.y;

        if (Mathf.Abs(dx) + Mathf.Abs(dy) == 1)
        {
            animator.SetTrigger("Attack");
            return;
        }

        Vector3Int moveTo = enemyPos;

        if (Mathf.Abs(dx) > Mathf.Abs(dy))
            moveTo.x += dx > 0 ? 1 : -1;
        else
            moveTo.y += dy > 0 ? 1 : -1;

        if (groundTilemap.HasTile(moveTo))
        {
            LastMoveTarget = moveTo;
            StartCoroutine(StartMovingEnemy());
            FaceDirection(player.transform.position);
        }
    }
    IEnumerator StartMovingEnemy()
    {
        Vector3 target = groundTilemap.GetCellCenterWorld(LastMoveTarget);

        animator.SetBool("IsMoving", true);

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
        animator.SetBool("IsMoving", false);
    }
    public void EnemyDealDamage()
    {
        if (playerScript != null)
        {
            playerScript.TakeDamage(damage);
            FaceDirection(player.transform.position);
        }
    }
    void FaceDirection(Vector3 target)
    {
        if (target.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (target.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
