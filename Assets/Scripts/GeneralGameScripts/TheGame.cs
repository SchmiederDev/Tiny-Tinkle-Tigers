using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class TheGame : MonoBehaviour
{   
    public static TheGame GameControl;
    
    public MapGrid GameGrid;        
    public GameTimer Timer { get; private set; }

    [SerializeField]
    private TimetickDisplay TimeDisplay;

    [SerializeField]
    private LevelGenerator lvlGenerator;

    [SerializeField]
    private ScreenFader LeakyKittensScreen;

    public AudioManager GameAudio;

    public int trainedKittens { get; set; } = 0;
    [SerializeField]
    private List<GameObject> Kittens_DB;
    public List<GameObject> KittensOnScene { get; private set; }
    public int kittensToTrain { get; private set; } 

    
    [SerializeField]
    private List<GameObject> MapObjects_DB;    
    public List<GameObject> MapObjectsOnScene { get; private set; }

    [SerializeField]
    private float upperMapBorder = 3f;
    [SerializeField]
    private float lowerMapBorder = -3f;
    [SerializeField]
    private float leftMapBorder = -7f;
    [SerializeField]
    private float rightMapBorder = 7f;

    [SerializeField]
    private GameObject CatTrayGoal;

    private GameObject CatTrayInstance;

    public bool gameCanStart { get; set; } = false;
    public bool levelGoalAccomplished { get; set; } = false;
    public bool newLevelIsLoading { get; set; } = false;

    public int PlayerScore { get; set; }

    [SerializeField]
    private TMP_Text PlayerScoreText;
    void Awake()
    {
        Initialize_GameControl();
        
        GameGrid = GetComponentInChildren<MapGrid>();
        Timer = GetComponent<GameTimer>();
        lvlGenerator = GetComponentInChildren<LevelGenerator>();
        GameAudio = GetComponentInChildren<AudioManager>();

        LeakyKittensScreen = GameObject.FindGameObjectWithTag("LeakyKittensScreen").GetComponent<ScreenFader>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForGameStart());
    }

    private void Initialize_GameControl()
    {
        if (GameControl == null)
            GameControl = this;

        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Init_GameStart()
    {
        GameAudio.Play_RandomSong();
        SpawnFirstKitten();

        KittensOnScene = new List<GameObject>();
        FindKittensOnScene();

        Timer.Init_Timer(KittensOnScene.Count);

        MapObjectsOnScene = new List<GameObject>();
        SpawnCatTray();        
        Init_MapObjects();
        gameCanStart = false;

    }

    private void SpawnFirstKitten()
    {
        int randomKittenIndex = UnityEngine.Random.Range(0, Kittens_DB.Count);
        Instantiate(Kittens_DB[randomKittenIndex], CalculateRandomPosition(), Quaternion.identity);
    }

    private void SpawnKittens()
    {
        for(int i = 0; i < lvlGenerator.targetKittenNumberOnScene; i++)
        {
            int randomKittenIndex = UnityEngine.Random.Range(0, Kittens_DB.Count);
            Instantiate(Kittens_DB[randomKittenIndex], CalculateRandomPosition(), Quaternion.identity);
        }
    }

    private Vector2 CalculateRandomPosition()
    {
        float xPos = Random.Range(leftMapBorder, rightMapBorder);
        float yPos = Random.Range(upperMapBorder, lowerMapBorder);
        
        Vector2 randomPos = new Vector2(xPos, yPos);
        return randomPos;
    }

    private void SpawnCatTray()
    {
        MapObject CatTray_MO = CatTrayGoal.GetComponent<MapObject>();
        CatTray_MO.Init_MapObject();
        
        Vector2 cellOrigin = CalculateMapObjectPosition(CatTray_MO.ObjectScale);
        
        CatTrayInstance = Instantiate(CatTrayGoal, cellOrigin, Quaternion.identity);
        MapObject CatTrayInst_MO = CatTrayInstance.GetComponent<MapObject>();
        CatTrayInst_MO.Init_MapObject();
        CatTrayInst_MO.CalculateRelatedGridCells();
        CatTrayInst_MO.LockGridCells();
        MapObjectsOnScene.Add(CatTrayInstance);

    }

    void Update()
    {
        
        if (!lvlGenerator.gameWon)
        {
            if (levelGoalAccomplished)
            {
                gameCanStart = false;

                if(newLevelIsLoading)
                {
                    GameAudio.Stop_BackgroundMusic();
                    levelGoalAccomplished = false;

                    TimeDisplay.timeDisplayHalt = true;

                    trainedKittens = 0;

                    lvlGenerator.ProgressInLevel();

                    Disable_KittenControls();
                    CleanUpScene();
                    Timer.StopAllCoroutines();
                    Timer.ResetTimer();

                    LeakyKittensScreen.StartFadeIn();
                }
            }

            if (gameCanStart)
            {
                GameAudio.Play_RandomSong();
                SpawnKittens();
                FindKittensOnScene();

                Timer.Init_Timer(KittensOnScene.Count);

                SpawnCatTray();
                Init_MapObjects();

                newLevelIsLoading = false;
                TimeDisplay.timeDisplayHalt = false;
                gameCanStart = false;
            }
        }
    }

    private void FindKittensOnScene()
    {
        GameObject[] kittensOnScene = GameObject.FindGameObjectsWithTag("Kitten");
        KittensOnScene.AddRange(kittensOnScene);
        kittensToTrain = KittensOnScene.Count;
    }

    private void Disable_KittenControls()
    {
        foreach(GameObject kitten in KittensOnScene)
        {
            KittenController kittenController = kitten.GetComponent<KittenController>();
            kittenController.controlsEnabled = false;
        }
    }

    private void CleanUpScene()
    {
        if(KittensOnScene.Count > 0)
            RemoveKittensFromScene();

        RemoveMapObjectsFromScene();
        ResetGridCells();
    }

    public void RemoveKittenFromScene(GameObject KittenToRemove)
    {
        KittensOnScene.Remove(KittenToRemove);
        Destroy(KittenToRemove);
    }

    private void RemoveKittensFromScene()
    {
        for(int i = KittensOnScene.Count; i > 0; i--)
        {
            GameObject CurrentKitten = KittensOnScene[i-1];
            KittensOnScene.RemoveAt(i-1);
            Destroy(CurrentKitten);
        }
    }

    private void RemoveMapObjectsFromScene()
    {
        for(int i = MapObjectsOnScene.Count; i > 0; i--)
        {
            GameObject MapObject = MapObjectsOnScene[i-1];
            MapObjectsOnScene.RemoveAt(i-1);
            Destroy (MapObject);
        }
    }

    private void ResetGridCells()
    {
        foreach(GameObject GridCell in GameGrid.GridCells)
        {
            GridCellGameObject CurrentGridCells  = GridCell.GetComponent<GridCellGameObject>();
            CurrentGridCells.isOccupied = false;
        }
    }

    #region MapObject Methods
    private void Init_MapObjects()
    {
        for(int i = 0; i < lvlGenerator.targetObjectNumberOnScene; i++)
        {   
            GameObject mapObject_GO = MapObjects_DB[GenerateRandomIndexForMapObject()];
            MapObject mapObject_MO = mapObject_GO.GetComponent<MapObject>();
            mapObject_MO.Init_MapObject();
            Vector2 cellOrigin = CalculateMapObjectPosition(mapObject_MO.ObjectScale);
            
            GameObject mapObject_GO_Inst = Instantiate(mapObject_GO, cellOrigin, Quaternion.identity);
            MapObject mapObject_MO_Inst = mapObject_GO_Inst.GetComponent<MapObject>();
            mapObject_MO_Inst.Init_MapObject();
            mapObject_MO_Inst.CalculateRelatedGridCells();

            if (MapObjectsOnScene.Count < 1)
                mapObject_MO_Inst.LockGridCells();

            else
            {
                CheckOverlap_and_Reposition(mapObject_GO_Inst);
                mapObject_MO_Inst.LockGridCells();
            }

            if (mapObject_GO_Inst != null)
                MapObjectsOnScene.Add(mapObject_GO_Inst);
        }
    }

    private int GenerateRandomIndexForMapObject()
    {
        int randomObjectIndex = UnityEngine.Random.Range(0, MapObjects_DB.Count - 1);

        if (randomObjectIndex >= 0 && randomObjectIndex < MapObjects_DB.Count)
            return randomObjectIndex;

        else
        {
            randomObjectIndex = 0;
            return randomObjectIndex;
        }

    }

    private Vector2 CalculateMapObjectPosition(Vector2 ObjectScale)
    {
        bool fitsLevelBorders = false;

        Vector2 finalPos;

        do
        {
            GameObject RandomCell = FetchRandomGridCell();
            GridCellGameObject GridCellOrigin = RandomCell.GetComponent<GridCellGameObject>();
                        
            Vector2 originPos = GridCellOrigin.Cell.Origin;
            finalPos = AdaptToSize(originPos, ObjectScale);
            
            float leftExtension = GridCellOrigin.Cell.UpperLeft.x;
            float rightExtension = GridCellOrigin.Cell.UpperLeft.x + ObjectScale.x;
            float upperExtension = GridCellOrigin.Cell.UpperLeft.y;
            float lowerExtension = GridCellOrigin.Cell.UpperLeft.y - ObjectScale.y;

            if (leftExtension >= leftMapBorder && rightExtension <= rightMapBorder && lowerExtension >= lowerMapBorder && upperExtension <= upperMapBorder)
                fitsLevelBorders = true;

        } while (!fitsLevelBorders);

        return finalPos;
    }    

    private GameObject FetchRandomGridCell()
    {
        int randomIndex = UnityEngine.Random.Range(0, GameGrid.GridCells.Count -1);
        GameObject gridCell = GameGrid.GridCells[randomIndex];
        return gridCell;
    }

    private Vector2 AdaptToSize(Vector2 OriginalPosition, Vector2 ObjectScale)
    {

        if (ObjectScale.x > 1f)
            OriginalPosition.x -= 0.5f;

        if (ObjectScale.y > 1f)
            OriginalPosition.y += 0.5f;

        return OriginalPosition;
    }

    private void CheckOverlap_and_Reposition(GameObject mapObjectToCheck)
    {
        MapObject AttachedMapObject = mapObjectToCheck.GetComponent<MapObject>();
        
        bool cellAlreadyOccupied = AttachedMapObject.RelatedGridCells.Find(relatedGridCell => relatedGridCell.isOccupied);

        if(cellAlreadyOccupied)
        {
            int callCounter = 0;

            Vector2 finalPosition = new Vector2();

            do
            {
                callCounter++;

                AttachedMapObject.RelatedGridCells.Clear();
                finalPosition = CalculateMapObjectPosition(AttachedMapObject.ObjectScale);
                mapObjectToCheck.transform.position = finalPosition;
                AttachedMapObject.CalculateRelatedGridCells();
                cellAlreadyOccupied = AttachedMapObject.RelatedGridCells.Find(relatedGridCell => relatedGridCell.isOccupied);

                if (callCounter > 20)
                {
                    Destroy(mapObjectToCheck);
                    break;
                }
                
            } while (cellAlreadyOccupied);
        }
    }

    #endregion

    public void AddXP(int xp)
    {
        long tempScore = PlayerScore + xp;
        
        if(tempScore < int.MaxValue)
        {
            PlayerScore += xp;
            UpdatePlayerScore();
        }
    }

    public void UpdatePlayerScore()
    {
        PlayerScoreText.text = PlayerScore.ToString();
    }

    private IEnumerator WaitForGameStart()
    {
        yield return new WaitForSeconds(0.01f);

        if(gameCanStart)
            Init_GameStart();

        else
            StartCoroutine(WaitForGameStart());

    }

    
}
