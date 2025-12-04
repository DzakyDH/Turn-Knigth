using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public bool isPlayerPhase = true;
    public List<PlayerGridMovement> allAllies = new List<PlayerGridMovement>();
    public List<EnemyDummy> allEnemies = new List<EnemyDummy>();
    public HashSet<Vector3Int> occupiedTiles = new HashSet<Vector3Int>();
    public Grid grid;
    public Tilemap groundTilemap;

    private int enemiesDone = 0;

    public bool IsTileOccupied(Vector3Int tile)
    {
        Vector3 world = grid.GetCellCenterWorld(tile);

        foreach (var ally in allAllies)

            if (Vector2.Distance(ally.transform.position, world) < 0.01f)
                return true;

        foreach (var enemy in allEnemies)
   
            if (Vector2.Distance(enemy.transform.position, world) < 0.01f)
                return true;

        return false;
    }    

    public bool IsTileOccupiedByAlly(Vector3Int tile)
    {
        Vector3 world = grid.GetCellCenterWorld(tile);

        foreach (var ally in allAllies)
            if (Vector2.Distance(ally.transform.position, world) < 0.01f)
                return true;

        return false;
    }
    public bool IsTileOccupiedByEnemy(Vector3Int tile)
    {
        Vector3 world = grid.GetCellCenterWorld(tile);
        foreach (var enemy in allEnemies)
            if (Vector2.Distance(enemy.transform.position, world) < 0.01f)
                return true;

        return false;
    }
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshAllUnits();
        StartPlayerPhase();
    }

    public void RefreshAllUnits()
    {
        allEnemies.Clear();
        allAllies.Clear();

        allAllies.AddRange(FindObjectsByType<PlayerGridMovement>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
        allEnemies.AddRange(FindObjectsByType<EnemyDummy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
    }

    public void StartPlayerPhase()
    {
        isPlayerPhase = true;
        
        foreach (var ally in allAllies)
        {
            ally.hasMoved = false;
        }
    }

    public void NotifyAllyFinished(PlayerGridMovement ally)
    {
        ally.hasMoved = true;

        foreach (var a in allAllies)
        {
            if (!a.hasMoved)
                return;
        }
        StartEnemyPhase();
    }
    public void StartEnemyPhase()
    {
        isPlayerPhase = false;

        StartCoroutine(EnemyPhaseCoroutine());
    }
    IEnumerator EnemyPhaseCoroutine()
    {
        RefreshAllUnits();

        enemiesDone = 0;

        foreach (var enemy in allEnemies)
        {
            enemy.EnemyTurn();
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void NotifyEnemyFinished()
    {
        enemiesDone++;

        if (enemiesDone >= allEnemies.Count)
        {
            enemiesDone = 0;
            StartPlayerPhase();
        }
    }
    public void RemoveAlly(PlayerGridMovement ally)
    {
        if (allAllies.Contains(ally))
            allAllies.Remove(ally);
    }
    public void RemoveEnemy(EnemyDummy enemy)
    {
        if (allEnemies.Contains(enemy))
            allEnemies.Remove(enemy);
    }
}
