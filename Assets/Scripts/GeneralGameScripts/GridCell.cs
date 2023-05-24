using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{    
    public Vector2 Origin { get; private set; }

    public Vector2 UpperLeft { get; private set; }
    public Vector2 LowerLeft { get; private set; }
    public Vector2 UpperRight { get; private set; }
    public Vector2 LowerRight { get; private set; }
    

    public int Row { get; set; }
    public int Col { get; set; }

    public GridCell(Vector2 origin, float cellSize)
    {
        Origin = origin;
        
        float halfCellSize = cellSize / 2f;

        UpperLeft = new Vector2(origin.x - halfCellSize, origin.y + halfCellSize);
        LowerLeft = new Vector2(origin.x - halfCellSize, origin.y - halfCellSize);
        UpperRight = new Vector2(origin.x + halfCellSize, origin.y + halfCellSize);
        LowerRight = new Vector2(origin.x + halfCellSize, origin.y - halfCellSize);
    }
}
