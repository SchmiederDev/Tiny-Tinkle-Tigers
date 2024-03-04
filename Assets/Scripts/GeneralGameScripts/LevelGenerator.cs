using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public int levelIndex { get; private set; } = 1;

    
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
    private int gameModesSpawnOnlyKittensThreshold = 10;

    [SerializeField]
    private int gameModesGameObjectsOnThresholdThreshold = 20;

    [SerializeField]
    private int gameModesSpawnBothThreshold = 30;

    // Start is called before the first frame update
    void Awake()
    {
        targetKittenNumberOnScene = MinKittensOnScene;
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
        if (levelIndex <= gameModesSpawnOnlyKittensThreshold)
            currentGameMode = GameMode.spawnOnlyKittens;

        else if (levelIndex > gameModesSpawnOnlyKittensThreshold && levelIndex <= gameModesGameObjectsOnThresholdThreshold)
            currentGameMode = GameMode.spawnMapObjectOnThreshold;

        else if (levelIndex > gameModesGameObjectsOnThresholdThreshold && levelIndex <= gameModesSpawnBothThreshold)
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

        Debug.Log($"Game mode: {currentGameMode}");

    }

    private void SpawnOnlyKittens()
    {
        SpawnKittens();
    }

    private void SpawnKittens()
    {
        int nextKittenNumberOnScene = targetKittenNumberOnScene + 1;

        if (nextKittenNumberOnScene <= MaxKittensOnScene)
        {
            targetKittenNumberOnScene++;
        }

        if (targetKittenNumberOnScene == MaxKittensOnScene)
            maxKittensReached = true;
    }

    private void SpawnMapObjectOnThreshold()
    {       

        if (maxKittensReached)
            ResetLevelParameters();

        else
        {
            SpawnKittens();
            SpawnObjectsOnThreshold();            
        }
    }

    private void SpawnObjectsOnThreshold()
    {
        int kittenThresholdmodifier = levelIndex % AddKittenThreshold;

        if (kittenThresholdmodifier == 0)
        {
            int nextMapObjectNumber = targetObjectNumberOnScene + 1;

            if (nextMapObjectNumber <= MaxObjectsOnScene)
                targetObjectNumberOnScene++;

        }
    }

    private void SpawnBothEqually()
    {        

        if (maxKittensReached)
            ResetLevelParameters();

        else
        {
            SpawnKittens();
            SpawnObjects();
        }

        CheckGameWinCondition();

    }

    private void SpawnObjects()
    {
        int nextMapObjectNumber = targetObjectNumberOnScene + 1;

        if (nextMapObjectNumber <= MaxObjectsOnScene)
            targetObjectNumberOnScene++;
    }

    private void ResetLevelParameters()
    {
        maxKittensReached = false;
        targetKittenNumberOnScene = MinKittensOnScene;
        targetObjectNumberOnScene = MinObjectsOnScene;
        
    }

    private void CheckGameWinCondition()
    {
        if (targetKittenNumberOnScene == MaxKittensOnScene && targetObjectNumberOnScene == MaxObjectsOnScene)
            gameWon = true;
    }

    enum GameMode
    {
        spawnOnlyKittens,
        spawnMapObjectOnThreshold,
        spawnBothEqually
    }

}
