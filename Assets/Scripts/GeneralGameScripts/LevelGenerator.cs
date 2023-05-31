using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public int levelIndex { get; private set; } = 0;

    
    public int MinKittensOnScene = 1;
    public int MaxKittensOnScene = 10;
    public int targetKittenNumberOnScene { get; private set; } = 1;

    public int AddKittenThreshold = 3;

    private bool maxKittensReached = false;
    public bool gameWon { get; private set; } = false;

    public int AddMapObjectThreshold = 5;

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
        if (targetKittenNumberOnScene == MaxKittensOnScene)
        {
            maxKittensReached = true;
            targetKittenNumberOnScene = MinKittensOnScene;
            targetObjectNumberOnScene = MinObjectsOnScene;
        }

        if (!maxKittensReached)
        {
            int nextKittenNumberOnScene = targetKittenNumberOnScene + 1;

            if (nextKittenNumberOnScene <= MaxKittensOnScene)
                targetKittenNumberOnScene++;

            int kittenThresholdmodifier = levelIndex % AddKittenThreshold;

            if (kittenThresholdmodifier == 0)
            {
                int nextMapObjectNumber = targetObjectNumberOnScene + 1;

                if (nextMapObjectNumber <= MaxObjectsOnScene)
                    targetObjectNumberOnScene++;

            }
        }

        else
        {
            int nextKittenNumberOnScene = targetKittenNumberOnScene + 1;

            if (nextKittenNumberOnScene <= MaxKittensOnScene)
                targetKittenNumberOnScene++;

            int nextMapObjectNumber = targetObjectNumberOnScene + 1;

            if (nextMapObjectNumber <= MaxObjectsOnScene)
                targetObjectNumberOnScene++;

        }

        if (targetKittenNumberOnScene > MaxKittensOnScene && targetObjectNumberOnScene > MaxObjectsOnScene)
            gameWon = true;


    }

}
