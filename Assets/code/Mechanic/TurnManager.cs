using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public List<CharacterBase> players = new List<CharacterBase>();
    public List<CharacterBase> enemies = new List<CharacterBase>();

    private void Awake() => Instance = this;

    private void Start() => StartCoroutine(MainTurnLoop());

    IEnumerator MainTurnLoop()
    {
        while (true)
        {
            // Player turn
            foreach (var p in players)
            {
                if (p == null || p.IsDead) continue;
                yield return StartCoroutine(p.TakeTurn());
            }

            // Enemy turn
            foreach (var e in enemies)
            {
                if (e == null || e.IsDead) continue;
                yield return StartCoroutine(e.TakeTurn());
            }
        }
    }
}