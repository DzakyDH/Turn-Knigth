using System.Collections;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public bool IsDead = false;
    public bool TurnDone = false;

    public virtual IEnumerator TakeTurn()
    {
        TurnDone = false;

        while (!TurnDone)
            yield return null;
    }

    public virtual void EndTurn()
    {
        TurnDone = true;
    }
}
