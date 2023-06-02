using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    Transform Grid;

    [SerializeField]
    private Sprite groundSprite;

    public List<GameObject> GridCells { get; private set; }

    [SerializeField]
    private Vector2 MapOrigin = new Vector2(-7.5f, 3.5f);
       

    [SerializeField]
    private float cellSize = 1f;

    [SerializeField]
    private int numberOfCellsOnXAxis = 16;

    [SerializeField]
    private int numberOfCellsOnYAxis = 8;

    private GridCell OriginalCell;

    private int row = 1;
    private int column = 1;


    private int gridCellCounter = 0;

    private void Awake()
    {
        Grid = GetComponent<Transform>();
        GridCells = new List<GameObject>();
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        InitializeFirstCell();
        FillYAxis();
    }

    private void InitializeFirstCell()
    {
        OriginalCell = new GridCell(MapOrigin, cellSize);
    }

    private void FillXAxis(Vector2 YOrigin)
    {
        float nextX = YOrigin.x - 1;

        for(int i = 0; i < numberOfCellsOnXAxis; i++)
        {            
            Vector2 nextOrigin = new Vector2(nextX + cellSize, YOrigin.y);      
            GridCell NextCell = new GridCell(nextOrigin, cellSize);
           
            GameObject GridCell = new GameObject();
            

            GridCell.AddComponent<BoxCollider2D>();
            GridCell.AddComponent<SpriteRenderer>();

            SpriteRenderer GridCellRenderer = GridCell.GetComponent<SpriteRenderer>();
            GridCellRenderer.sprite = groundSprite;

            GridCell.AddComponent<GridCellGameObject>();
           
            GridCellGameObject GridCellObject = GridCell.GetComponent<GridCellGameObject>();
            GridCellObject.Initialize_GridCellObject(NextCell, new Vector2(cellSize, cellSize));

            GridCellObject.Cell.Row = row;
            GridCellObject.Cell.Col = column;            

            GridCell.transform.position = NextCell.Origin;
            GridCell.transform.parent = Grid;

            gridCellCounter++;
            
            GridCell.name = "Grid Cell: " + gridCellCounter + " - row " + GridCellObject.Cell.Row.ToString() + ", column: " + GridCellObject.Cell.Col.ToString();

            GridCellGameObject GridCellToName = GridCell.GetComponent<GridCellGameObject>();
            GridCellToName.cellName = gridCellCounter.ToString();


            GridCells.Add(GridCell);

            nextX += cellSize;
            column++;            
        }
    }

    private void FillYAxis()
    {        
        Vector2 YOrigin = OriginalCell.Origin;

        for(int i = 0; i < numberOfCellsOnYAxis; i++)
        {            
            FillXAxis(YOrigin);
            YOrigin.y--;
            row++;
            column = 1;
        }
    }

    public GridCellGameObject Find_GridCell_byPosition(Vector2 objectCorner)
    {
        /* This laborious code is introduced because the 'GridCellGameObject'-Class inherits from MonoBehaviour
           because the attached collider is accessed through its members.
           Since a new instance of a class that inherits from MonoBehaviour may not be initialized with the 'new'-Keyword
           the instance gets a default value (the first of all grid cells) which is then overwritten.
        */

        GridCellGameObject targetCell = GridCells[0].GetComponent<GridCellGameObject>();
        bool isDefaultCell = true;

        foreach(GameObject GridCell in GridCells)
        {
            GridCellGameObject CurrentGridCell = GridCell.GetComponent<GridCellGameObject>();
            
            if(CurrentGridCell.Cell.UpperLeft == objectCorner)
            {
                targetCell = CurrentGridCell;
                isDefaultCell = false;
            }
        }

        if (!isDefaultCell && targetCell != null)
            return targetCell;
        else
            return null;
    }
}
