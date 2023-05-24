using UnityEngine;

public class GridCellGameObject : MonoBehaviour
{
    public string cellName { get; set; }
    public GridCell Cell { get; private set; }

    public BoxCollider2D CellCollider { get; private set; }
    
    Vector2 ColliderSize;

    bool isOccupied = false;

    public delegate void OnCellStatusChanged();
    public OnCellStatusChanged onCellStatusChanged;

    private void Awake()
    {
        CellCollider = GetComponent<BoxCollider2D>();
        
    }

    private void Start()
    {
        onCellStatusChanged += ShowCellStatus;
    }

    public void Initialize_GridCellObject(GridCell myCell, Vector2 colliderSize)
    {
        Cell = myCell;
        CellCollider.size = colliderSize;
        CellCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidingObject = collision.gameObject;

        if (collidingObject.tag == "Furniture")
        {
            isOccupied = true;
            //onCellStatusChanged.Invoke();
        }

        //if (collidingObject.tag == "Kitten")
        //    Debug.Log("Grid Cell No.: " + cellName + " Kitten walked over me.");
    }

    public void ShowCellStatus()
    {
        Debug.Log("Cell " + cellName + " is occupied: " + isOccupied);
    }
}
