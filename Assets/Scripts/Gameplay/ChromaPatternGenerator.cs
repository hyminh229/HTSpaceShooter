using UnityEngine;

public static class ChromaPatternGenerator
{
    // Sinh danh sách màu theo cụm (VD: BLUE BLUE BLUE RED RED BLUE RED RED RED...)
    // thay vì luân phiên đều đặn BLUE-RED-BLUE-RED dễ đoán.
    public static ElementColor[] GenerateClusterPattern(int count, int minClusterSize = 2, int maxClusterSize = 4)
    {
        ElementColor[] pattern = new ElementColor[count];
        int index = 0;
        ElementColor currentColor = Random.value < 0.5f ? ElementColor.BLUE : ElementColor.RED;

        while (index < count)
        {
            int clusterSize = Random.Range(minClusterSize, maxClusterSize + 1);

            for (int i = 0; i < clusterSize && index < count; i++)
            {
                pattern[index] = currentColor;
                index++;
            }

            currentColor = currentColor == ElementColor.BLUE ? ElementColor.RED : ElementColor.BLUE;
        }

        return pattern;
    }
}