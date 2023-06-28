using System.Collections;
using System.Collections.Generic;
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
    private ScreenFader TinkleTigersScreen;

    public AudioManager GameAudio;

    public int trainedKittens { get; set; } = 0;
    [SerializeField]
    private List<GameObject> Kittens_DB;
    public List<GameObject> KittensOnScene { get; private set; }
    public int kittensToTrain { get; private set; } 

    
    [SerializeField]
    private List<GameObject> MapObjects_DB;    
    public List<GameObject> MapObjectsOnScene { get; private set; }

    public List<GameObject> PuddlesOnScene { get; set; }

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

    public bool restartOnTimeOut { get; set; } = false;

    public int PlayerScore { get; set; }

    [SerializeField]
    private TMP_Text PlayerScoreText;

    public PopMessage GameNotification;

    void Awake()
    {
        Initialize_GameControl();
        
        GameGrid = GetComponentInChildren<MapGrid>();
        Timer = GetComponent<GameTimer>();
        lvlGenerator = GetComponentInChildren<LevelGenerator>();
        GameAudio = GetComponentInChildren<AudioManager>();

        TinkleTigersScreen = GameObject.FindGameObjectWithTag("TinkleTigersScreen").GetComponent<ScreenFader>();
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
        KittensOnScene = new List<GameObject>();
        MapObjectsOnScene = new List<GameObject>();
        PuddlesOnScene = new List<GameObject>();

        StartLevel();

    }

    private void SpawnKittens()
    {
        for (int i = 0; i < lvlGenerator.targetKittenNumberOnScene; i++)
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
                Disable_KittenControls();

                if (newLevelIsLoading)
                {
                    lvlGenerator.ProgressInLevel();
                    LoadLevel();                    
                }
            }

            if (gameCanStart)
                StartLevel();
        }
    }

    private void StartLevel()
    {
        newLevelIsLoading = false;       
        gameCanStart = false;

        SpawnKittens();
        FindKittensOnScene();

        if(!restartOnTimeOut)
        {
            SpawnCatTray();
            Init_MapObjects();
        }

        Timer.Init_Timer(KittensOnScene.Count);
        TimeDisplay.timeDisplayHalt = false;

        GameAudio.Play_RandomSong();
        
        restartOnTimeOut = false;
    }

    private void LoadLevel()
    {        
        levelGoalAccomplished = false;
        trainedKittens = 0;

        ResetGameTimer();
        CleanUpScene();

        GameAudio.Stop_BackgroundMusic();
        TinkleTigersScreen.StartFadeIn();
    }

    public void RestartOnTimeOut()
    {
        restartOnTimeOut = true;
        SendTimeOutMessage();
        StartCoroutine(WaitForTimeOutMessage());     
    }

    private void SendTimeOutMessage()
    {
        string timeOutMessage = GameNotification.gameNotesDB.FindNotification("TimeOut");
        GameNotification.SendPopMessage(timeOutMessage);
    }

    IEnumerator WaitForTimeOutMessage()
    {
        yield return new WaitForSeconds(0.1f);

        if (GameNotification.messagePlaying)
            StartCoroutine(WaitForTimeOutMessage());
        else
            LoadLevel();
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

    private void ResetGameTimer()
    {
        TimeDisplay.timeDisplayHalt = true;
        Timer.StopAllCoroutines();
        Timer.ResetTimer();
    }

    private void CleanUpScene()
    {
        if(KittensOnScene.Count > 0)
            RemoveKittensFromScene();
        if(PuddlesOnScene.Count > 0)
            RemovePuddlesFromScene();

        if(!restartOnTimeOut)
        {
            RemoveMapObjectsFromScene();
            ResetGridCells();
        }
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

    private void RemovePuddlesFromScene()
    {
        foreach(GameObject Puddle in PuddlesOnScene)
        {
            PeePuddle attachedPeePuddle = Puddle.GetComponent<PeePuddle>();

            if (attachedPeePuddle != null)
                StartCoroutine(attachedPeePuddle.DealPenalty());
            else
                Debug.Log("Puddle component not found");
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
        
        for (int i = 0; i < lvlGenerator.targetObjectNumberOnScene; i++)
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

    public void SubstractXP(int xp)
    {
        long tempScore = PlayerScore - xp;

        if (tempScore > 0)
        {
            PlayerScore -= xp;
            UpdatePlayerScore();
        }

        else
        {
            PlayerScore = 0;
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
