using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    [SerializeField]
    private Furniture furnitureObject;
    public string ObjectName { get; private set; }

    public GridCellGameObject CellOrigin { get; private set; }
    public List<GridCellGameObject> RelatedGridCells { get; private set; }
    public bool hasEmptyCellOrigin { get; private set; } = true;

    public Vector2 ObjectUpperLeft { get; private set; }
    public Vector2 ObjectScale { get; private set; }

    [SerializeField]
    private float minimalDistance = 1f;

    public float effectiveDistance { get; private set; }


    [SerializeField]
    private BoxCollider2D ObjectCollider;

    // Start is called before the first frame update
    public void Init_MapObject()
    {
        RelatedGridCells = new List<GridCellGameObject>();
        Get_FurnitureObject_and_Set_Scale();
        CalculateEffectiveDistance();
    }

    private void Get_FurnitureObject_and_Set_Scale()
    {        
        ObjectName = furnitureObject.ObjectName;
        ObjectScale = new Vector2(furnitureObject.X_Extension, furnitureObject.Y_Extension);
    }

    private void CalculateEffectiveDistance()
    {
        effectiveDistance = ObjectScale.x + minimalDistance;
    }

    private void Calculate_UpperLeft()
    {
        float xPos = gameObject.transform.position.x - (ObjectScale.x/2f);
        float yPos = gameObject.transform.position.y + (ObjectScale.y/2f);
        ObjectUpperLeft = new Vector2(xPos, yPos);
    }

    public void CalculateRelatedGridCells()
    {
        Calculate_UpperLeft();
        FindOrigin();
        CalculateAdjacentGridCells();
    }

    public void LockGridCells()
    {
        foreach(GridCellGameObject RelatedGridcell in RelatedGridCells)
        {
            RelatedGridcell.isOccupied = true;
        }
    }

    private void FindOrigin()
    {
        CellOrigin = TheGame.GameControl.GameGrid.Find_GridCell_byPosition(ObjectUpperLeft);
        
        if(CellOrigin != null)
        {
            hasEmptyCellOrigin = false;
            RelatedGridCells.Add(CellOrigin);
        }

        else
            hasEmptyCellOrigin = true;
    }

    private void CalculateAdjacentGridCells()
    {
        if(!hasEmptyCellOrigin)
        {
            float nextXCorner = ObjectUpperLeft.x + 1;
            float nextYCorner = ObjectUpperLeft.y - 1;

            if (ObjectScale.x > 1)
            {
                GridCellGameObject AdjacentXCell = TheGame.GameControl.GameGrid.Find_GridCell_byPosition(new Vector2(nextXCorner, ObjectUpperLeft.y));

                if (AdjacentXCell != null)
                {
                    RelatedGridCells.Add(AdjacentXCell);
                }
            }

            if (ObjectScale.y > 1)
            {                
                GridCellGameObject AdjacentYCell = TheGame.GameControl.GameGrid.Find_GridCell_byPosition(new Vector2(ObjectUpperLeft.x, nextYCorner));

                if (AdjacentYCell != null)
                {
                    RelatedGridCells.Add(AdjacentYCell);
                }

                if (ObjectScale.x > 1)
                {
                    GridCellGameObject AdjacentXYCell = TheGame.GameControl.GameGrid.Find_GridCell_byPosition(new Vector2(nextXCorner, nextYCorner));

                    if (AdjacentXYCell != null)
                    {                        
                        RelatedGridCells.Add(AdjacentXYCell);
                    }
                }
            }
        }
    }

}
