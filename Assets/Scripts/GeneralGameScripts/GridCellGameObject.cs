using UnityEngine;

public class GridCellGameObject : MonoBehaviour
{
    public string cellName { get; set; }
    public GridCell Cell { get; private set; }

    public BoxCollider2D CellCollider { get; private set; }
    
    Vector2 ColliderSize;

    public bool isOccupied = false;

    public delegate void OnCellStatusChanged();
    public OnCellStatusChanged onCellStatusChanged;

    private void Awake()
    {
        CellCollider = GetComponent<BoxCollider2D>();
        onCellStatusChanged += UpdateCellStatus;
    }

    public void Initialize_GridCellObject(GridCell myCell, Vector2 colliderSize)
    {
        Cell = myCell;
        CellCollider.size = colliderSize;
        CellCollider.isTrigger = true;
    }

    private void UpdateCellStatus()
    {
        isOccupied = !isOccupied;
    }
}
