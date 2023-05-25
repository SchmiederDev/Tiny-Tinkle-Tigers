using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public int levelIndex { get; private set; } = 0;

    public int targetKittenNumberOnScene { get; private set; } = 1;
    public int MaxKittensOnScene = 10;

    public int AddKittenThreshold = 3;

    public int MinObjectsOnScene = 2;    
    public int MaxObjectsOnScene = 10;

    public int targetObjectNumberOnScene { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        targetObjectNumberOnScene = MinObjectsOnScene;
    }

    public void ProgressInLevel()
    {
        levelIndex++;
        CalculateNextLevelParameters();
    }

    private void CalculateNextLevelParameters()
    {
        int kittenThresholdCounter = levelIndex % AddKittenThreshold;

        if (kittenThresholdCounter == 0)
            targetKittenNumberOnScene++;

        targetObjectNumberOnScene++;
    }
}
