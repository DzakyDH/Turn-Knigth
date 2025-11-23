using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public bool isPlayerTurn = true;

    private List<EnemyDummy> allEnemies = new List<EnemyDummy>();

    void Start()
    {
        RefreshEnemyList();
    }

    public void RefreshEnemyList()
    {
        allEnemies.Clear();
        allEnemies.AddRange(FindObjectsByType<EnemyDummy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
    }

    public void EndPlayerTurn()
    {
        isPlayerTurn = false;
        StartCoroutine(EnemyTurnAll());
    }

    IEnumerator EnemyTurnAll()
    {
        RefreshEnemyList();

        foreach (EnemyDummy enemy in allEnemies)
        {
            enemy.EnemyTurn();
            yield return new WaitForSeconds(0.4f);
        }

        // Kembali ke Player
        isPlayerTurn = true;
    }
}
