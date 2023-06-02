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
    private bool maxObjectReached = false;
    public bool gameWon { get; private set; } = false;

    public int AddMapObjectThreshold = 5;

    public int MinObjectsOnScene = 2;
    public int MaxObjectsOnScene = 10;

    public int targetObjectNumberOnScene { get; private set; }

    private GameMode currentGameMode = GameMode.spawnMapObjects;

    [SerializeField]
    private int gameModesSpawnMapObjectsThreshold = 10;

    [SerializeField]
    private int gameModesSpawnKittensThreshold = 20;

    [SerializeField]
    private int gameModesSpawnBothThreshold = 30;

    // Start is called before the first frame update
    void Awake()
    {
        targetObjectNumberOnScene = MinObjectsOnScene;
    }

    public void ProgressInLevel()
    {
        levelIndex++;
        CheckLevelState();
        CalculateNextLevelParameters();
    }

    private void CheckLevelState()
    {
        if (levelIndex <= gameModesSpawnMapObjectsThreshold)
            currentGameMode = GameMode.spawnMapObjects;

        else if (levelIndex > gameModesSpawnMapObjectsThreshold && levelIndex <= gameModesSpawnKittensThreshold)
            currentGameMode = GameMode.spawnKittens;

        else if (levelIndex > gameModesSpawnKittensThreshold && levelIndex <= gameModesSpawnBothThreshold)
            currentGameMode = GameMode.spawnBoth;
    }

    private void CalculateNextLevelParameters()
    {
        switch(currentGameMode)
        {
            case GameMode.spawnMapObjects:
                SpawnMapObjects();
                break;

            case GameMode.spawnKittens:
                SpawnKittens();
                break;

            case GameMode.spawnBoth:
                SpawnBoth();
                break;
        }


    }

    public void SpawnMapObjects()
    {
        if (targetObjectNumberOnScene == MaxObjectsOnScene)
            maxObjectReached = true;

        if (!maxObjectReached)
        {
            int nextMapObjectNumber = targetObjectNumberOnScene + 1;

            if (nextMapObjectNumber <= MaxObjectsOnScene)
                targetObjectNumberOnScene++;
        }

        else
            targetObjectNumberOnScene = MinObjectsOnScene;
    }

    public void SpawnKittens()
    {
        if (targetKittenNumberOnScene == MaxKittensOnScene)
            maxKittensReached = true;

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
            targetKittenNumberOnScene = MinKittensOnScene;
            targetObjectNumberOnScene = MinObjectsOnScene;
        }
    }

    private void SpawnBoth()
    {
        int nextKittenNumberOnScene = targetKittenNumberOnScene + 1;

        if (nextKittenNumberOnScene <= MaxKittensOnScene)
            targetKittenNumberOnScene++;

        int nextMapObjectNumber = targetObjectNumberOnScene + 1;

        if (nextMapObjectNumber <= MaxObjectsOnScene)
            targetObjectNumberOnScene++;

        if (targetKittenNumberOnScene > MaxKittensOnScene && targetObjectNumberOnScene > MaxObjectsOnScene)
            gameWon = true;
    }

    enum GameMode
    {
        spawnMapObjects,
        spawnKittens,
        spawnBoth
    }

}
