using UnityEngine;

public class PlayerContactDamage : MonoBehaviour
{
    [SerializeField] private int enemyContactDamage = 2;
    [SerializeField] private int meteorContactDamage = 3;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyController enemy))
        {
            playerHealth.TakeDamage(enemyContactDamage);
            enemy.DestroyObject();

            Debug.Log("Player va chạm Enemy! -" + enemyContactDamage + " HP.");
        }
        else if (collision.TryGetComponent(out MeteorController meteor))
        {
            playerHealth.TakeDamage(meteorContactDamage);

            Debug.Log("Player va chạm Meteor! -" + meteorContactDamage + " HP.");
        }
    }
}