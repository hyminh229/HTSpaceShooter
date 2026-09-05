using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;
    public bool IsAlive { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
        IsAlive = true;
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive)
        {

            return;
        }

        if (damage <= 0)
        {
            return;
        }
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. HP: " + currentHealth + "/" + maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        IsAlive = false;

        currentHealth = 0;

        Debug.Log(
            gameObject.name + " destroyed."
        );

        Destroy(gameObject);
    }
}