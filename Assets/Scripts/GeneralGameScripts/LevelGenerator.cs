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

    public int MinObjectsOnScene = 3;
    public int MaxObjectsOnScene = 10;

    public int targetObjectNumberOnScene { get; private set; }

    private GameMode currentGameMode = GameMode.spawnOnlyKittens;

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
            currentGameMode = GameMode.spawnOnlyKittens;

        else if (levelIndex > gameModesSpawnMapObjectsThreshold && levelIndex <= gameModesSpawnKittensThreshold)
            currentGameMode = GameMode.spawnMapObjectOnThreshold;

        else if (levelIndex > gameModesSpawnKittensThreshold && levelIndex <= gameModesSpawnBothThreshold)
            currentGameMode = GameMode.spawnBothEqually;
    }

    private void CalculateNextLevelParameters()
    {
        switch(currentGameMode)
        {
            case GameMode.spawnOnlyKittens:
                SpawnOnlyKittens();
                break;

            case GameMode.spawnMapObjectOnThreshold:
                SpawnMapObjectOnThreshold();
                break;

            case GameMode.spawnBothEqually:
                SpawnBothEqually();
                break;
        }


    }

    public void SpawnOnlyKittens()
    {
        if (!maxKittensReached)
        {
            int nextKittenNumberOnScene = targetKittenNumberOnScene + 1;

            if (nextKittenNumberOnScene <= MaxKittensOnScene)
                targetKittenNumberOnScene++;

            if (targetKittenNumberOnScene == MaxKittensOnScene)
                maxKittensReached = true;

        }

        else
            targetKittenNumberOnScene = MinKittensOnScene;
    }

    public void SpawnMapObjectOnThreshold()
    {
        if (!maxKittensReached)
        {
            int nextKittenNumberOnScene = targetKittenNumberOnScene + 1;

            if (nextKittenNumberOnScene <= MaxKittensOnScene)
                targetKittenNumberOnScene++;

            if (targetKittenNumberOnScene == MaxKittensOnScene)
                maxKittensReached = true;

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

    private void SpawnBothEqually()
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
        spawnOnlyKittens,
        spawnMapObjectOnThreshold,
        spawnBothEqually
    }

}
