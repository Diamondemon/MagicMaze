using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid 
{
    public int width, height;
    public float cellSize = 1;
    
    public Square[,] gridArray;


    public Grid(int width, int height)
    {
        this.width = width;
        this.height = height;
        this.cellSize = 1;



        gridArray = new Square[width, height];

        for (int x=0; x<gridArray.GetLength(0); x++){
            for (int y=0;y<gridArray.GetLength(1); y++){ 
                Debug.DrawLine(new Vector3(x,0.1f,y), new Vector3(x,0.1f,y+1), Color.white, 100f);
                Debug.DrawLine(new Vector3(x,0.1f,y), new Vector3(x+1,0.1f,y), Color.white, 100f);
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x,0.1f,y) * cellSize;
    }

    internal Square GetSquare(int x, int y)
    {
        if(x>=0 && x <width && y>=0 && y<height)
        {
            return gridArray[x,y];
        } 
        else { return null; }
    }
}
