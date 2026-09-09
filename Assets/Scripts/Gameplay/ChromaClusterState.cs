using UnityEngine;

public class ChromaClusterState
{
    private readonly int minClusterSize;
    private readonly int maxClusterSize;

    private ElementColor currentColor;
    private int remainingInCluster;

    public ChromaClusterState(int minClusterSize = 2, int maxClusterSize = 4)
    {
        this.minClusterSize = minClusterSize;
        this.maxClusterSize = maxClusterSize;

        currentColor = Random.value < 0.5f ? ElementColor.BLUE : ElementColor.RED;
        remainingInCluster = Random.Range(minClusterSize, maxClusterSize + 1);
    }

    public ElementColor Next()
    {
        if (remainingInCluster <= 0)
        {
            currentColor = currentColor == ElementColor.BLUE ? ElementColor.RED : ElementColor.BLUE;
            remainingInCluster = Random.Range(minClusterSize, maxClusterSize + 1);
        }

        remainingInCluster--;
        return currentColor;
    }
}