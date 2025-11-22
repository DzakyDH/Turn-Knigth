using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyDummy : MonoBehaviour
{
    public int hp = 3;
    public int damage = 1;
    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;
    public Transform player;

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
            player.GetComponent<PlayerGridMovement>().TakeDamage(damage);
            return;
        }
        Vector3Int moveTo = enemyPos;
        if (Mathf.Abs(dx) > Mathf.Abs(dy))
            moveTo.x += dx > 0 ? 1 : -1;
        else
            moveTo.y += dy > 0 ? 1 : -1;

        if (groundTilemap.HasTile(moveTo) && !obstacleTilemap.HasTile(moveTo))
        {
            transform.position = groundTilemap.GetCellCenterWorld(moveTo);
        }
    }
}
