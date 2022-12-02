using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid 
{
    private int width, height;
    private float cellSize;
    
    private Tile[,] gridArray;

    public Grid(int x, int y, float cellSize)
    {
        this.width = x;
        this.height = y;
        this.cellSize = cellSize;

        gridArray = new Tile[x, y];
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x,y) * cellSize;
    }

}
