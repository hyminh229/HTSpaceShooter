using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyEnergyDrop : MonoBehaviour
{
    [SerializeField] private GameObject energyOrbPrefab;

    private EnemyHealth enemyHealth;
    private EnemyPolarity enemyPolarity;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyPolarity = GetComponent<EnemyPolarity>();
    }

    private void OnEnable()
    {
        enemyHealth.OnDeath += DropOrb;
    }

    private void OnDisable()
    {
        enemyHealth.OnDeath -= DropOrb;
    }

    private void DropOrb()
    {
        if (energyOrbPrefab == null)
        {
            Debug.LogWarning("EnemyEnergyDrop is missing energyOrbPrefab reference.");
            return;
        }

        if (enemyPolarity == null)
        {
            Debug.LogWarning("EnemyEnergyDrop requires EnemyPolarity to know drop color.");
            return;
        }

        GameObject orbObject = Instantiate(energyOrbPrefab, transform.position, Quaternion.identity);

        if (orbObject.TryGetComponent(out EnergyOrb orb))
        {
            orb.SetColor(enemyPolarity.CurrentColor);
        }
    }
}