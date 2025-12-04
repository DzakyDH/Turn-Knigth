using System.Collections;
using System.Collections.Generic;
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

    private bool isActing = false;

    Transform GetClosestTarget()
    {
        float minDist = Mathf.Infinity;
        Transform closest = null;

        var allies = FindObjectsByType<PlayerGridMovement>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var a in allies)
        {
            float d = Vector2.Distance(transform.position, a.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = a.transform;
            }
        }
        return closest;
    }
    private void Start()
    {
        animator = GetComponent<Animator>();

        if (player != null)
            playerScript = player.GetComponent<PlayerGridMovement>();
    }
    public void EnemyTurn()
    {
        Transform target = GetClosestTarget();
        if (target == null)
        {
            EndTurn();
            return;
        }
        if (isActing) return;
        isActing = true;

        Vector3Int enemyPos = groundTilemap.WorldToCell(transform.position);
        Vector3Int targetPos = groundTilemap.WorldToCell(player.position);

        int dx = targetPos.x - enemyPos.x;
        int dy = targetPos.y - enemyPos.y;

        if (Mathf.Abs(dx) + Mathf.Abs(dy) == 1)
        {
            animator.SetTrigger("Attack");
            FaceDirection(player.position);
            return;
        }
        Vector3Int moveTo = enemyPos;
        if (Mathf.Abs(dx) > Mathf.Abs(dy))
            moveTo.x += dx > 0 ? 1 : -1;
        else
            moveTo.y += dy > 0 ? 1 : -1;

        if (groundTilemap.HasTile(moveTo) && !IsTileBlocked(moveTo))
        {
            LastMoveTarget = moveTo;
            StartCoroutine(StartMovingEnemy());
            FaceDirection(player.position);
        }
        else
        {
            EndTurn();
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

        EndTurn();
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            TurnManager.Instance.RemoveEnemy(this);
            Destroy(gameObject);
        }
    }
    public void EnemyDealDamage()
    {
        if (playerScript != null)
        {
            playerScript.TakeDamage(damage);
            FaceDirection(player.transform.position);
        }
        EndTurn();
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
    void EndTurn()
    {
        isActing = false;
        TurnManager.Instance.NotifyEnemyFinished();
    }
    bool IsTileBlocked(Vector3Int tile)
    {
        return TurnManager.Instance.IsTileOccupied(tile);
    }

}
