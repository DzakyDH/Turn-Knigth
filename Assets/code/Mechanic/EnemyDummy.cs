using UnityEngine;

public class EnemyDummy : MonoBehaviour
{
    public int hp = 3;

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        Debug.Log("Enemy Hp:" + hp);
         
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
