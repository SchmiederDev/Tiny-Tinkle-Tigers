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
    private LevelGenerator lvlGenerator;

    [SerializeField]
    private ScreenFader LeakyKittensScreen;

    public int trainedKittens { get; set; } = 0;
    [SerializeField]
    private List<GameObject> Kittens_DB;
    public List<GameObject> KittensOnScene { get; private set; }

    
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

    public bool gameCanStart { get; private set; } = false;
    public bool levelGoalAccomplished { get; set; } = false;
    public bool newLevelIsLoading { get; private set; } = false;

    public int PlayerScore { get; set; }

    [SerializeField]
    private TMP_Text PlayerScoreText;

    void Awake()
    {
        Initialize_GameControl();
        
        GameGrid = GetComponentInChildren<MapGrid>();
        Timer = GetComponent<GameTimer>();
        lvlGenerator = GetComponentInChildren<LevelGenerator>();

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
        SpawnFirstKitten();

        KittensOnScene = new List<GameObject>();
        FindKittensOnScene();

        Timer.Init_Timer(KittensOnScene.Count);

        MapObjectsOnScene = new List<GameObject>();
        Init_MapObjects();

        SpawnCatTray();
    }

    private void SpawnFirstKitten()
    {
        int randomKittenIndex = UnityEngine.Random.Range(0, Kittens_DB.Count);
        Instantiate(Kittens_DB[randomKittenIndex]);
    }

    private void SpawnCatTray()
    {
        float randomXpos = UnityEngine.Random.Range(leftMapBorder, rightMapBorder);
        float randomYpos = UnityEngine.Random.Range(lowerMapBorder, upperMapBorder);
        
        Vector2 StartingPos = new Vector2(randomXpos, randomYpos);

        CatTrayInstance = Instantiate(CatTrayGoal, StartingPos, Quaternion.identity);
    }

    void Update()
    {
        if (levelGoalAccomplished)
        {
            newLevelIsLoading = true;
            levelGoalAccomplished = false;

            Disable_KittenControls();
            LeakyKittensScreen.StartFadeIn();
            
            CleanUpScene();
        }
    }

    private void FindKittensOnScene()
    {
        GameObject[] kittensOnScene = GameObject.FindGameObjectsWithTag("Kitten");
        KittensOnScene.AddRange(kittensOnScene);
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
        RemoveKittensFromScene();
        RemoveMapObjectsFromScene();
        Destroy(CatTrayInstance);
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
            
            if(mapObject_GO_Inst != null)
                MapObjectsOnScene.Add(mapObject_GO_Inst);

            if (MapObjectsOnScene.Count > 1)
                CheckOverlap(mapObject_GO_Inst);
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

    private void CheckOverlap(GameObject mapObjectToCheck)
    {
        foreach(GameObject objectOnScene in MapObjectsOnScene)
        {
            if(objectOnScene != mapObjectToCheck)
            {
                float distance = Vector2.Distance(objectOnScene.transform.position, mapObjectToCheck.transform.position);

                MapObject mapObjectAlreadyOnScene = objectOnScene.GetComponent<MapObject>();

                if (distance < mapObjectAlreadyOnScene.effectiveDistance)
                {
                    
                    float shiftX = mapObjectToCheck.transform.position.x + mapObjectAlreadyOnScene.effectiveDistance;

                    if (shiftX < rightMapBorder)
                        mapObjectToCheck.transform.position = new Vector2(shiftX, mapObjectToCheck.transform.position.y);

                    else
                    {
                        float leftShift = -shiftX;
                        
                        if(leftShift > leftMapBorder)
                            mapObjectToCheck.transform.position = new Vector2(leftShift, mapObjectToCheck.transform.position.y);

                        else
                        {
                            if (mapObjectAlreadyOnScene.ObjectScale.y <= mapObjectAlreadyOnScene.ObjectScale.x)
                            {
                                float shiftY = mapObjectToCheck.transform.position.y + mapObjectAlreadyOnScene.effectiveDistance;
                                
                                if(shiftY < upperMapBorder)
                                    mapObjectToCheck.transform.position = new Vector2(mapObjectToCheck.transform.position.x, shiftY);

                                else
                                {
                                    shiftY *= -1f;

                                    if (shiftY > lowerMapBorder)
                                        mapObjectToCheck.transform.position = new Vector2(mapObjectToCheck.transform.position.x, shiftY);
                                    else
                                    {
                                        MapObjectsOnScene.Remove(mapObjectToCheck);
                                        Destroy(mapObjectToCheck);
                                    }
                                }
                            }

                            else
                            {
                                float shiftY = mapObjectToCheck.transform.position.y + mapObjectAlreadyOnScene.ObjectScale.y;

                                if (shiftY < upperMapBorder)
                                    mapObjectToCheck.transform.position = new Vector2(mapObjectToCheck.transform.position.x, shiftY);
                                else
                                    shiftY *= -1f;

                                if (shiftY > lowerMapBorder)
                                    mapObjectToCheck.transform.position = new Vector2(mapObjectToCheck.transform.position.x, shiftY);

                                else
                                {
                                    MapObjectsOnScene.Remove(mapObjectToCheck);
                                    Destroy(mapObjectToCheck);
                                }
                            }
                        }
                    }
                }
            }
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

        if(!LeakyKittensScreen.screenIsVisible)
            gameCanStart = true;

        if(gameCanStart)
            Init_GameStart();

        else
            StartCoroutine(WaitForGameStart());

    }

    
}
