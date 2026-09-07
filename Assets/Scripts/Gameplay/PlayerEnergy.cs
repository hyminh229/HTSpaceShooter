using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Energy")]
    [SerializeField] private int maxEnergy = 100;
    [SerializeField] private int currentEnergy = 0;

    public int CurrentEnergy => currentEnergy;
    public int MaxEnergy => maxEnergy;
    public bool IsFull => currentEnergy >= maxEnergy;

    public void AddEnergy(int amount)
    {
        if (amount <= 0) return;

        currentEnergy += amount;

        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }

        Debug.Log("Energy: " + currentEnergy + "/" + maxEnergy);
    }

    public bool UseMegaBeam()
    {
        if (!IsFull) return false;

        currentEnergy = 0;
        Debug.Log("Mega Beam used! Energy reset to 0.");

        return true;
    }
}