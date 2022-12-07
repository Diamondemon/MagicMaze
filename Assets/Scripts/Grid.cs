using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid 
{
    public int width, height;
    public float cellSize;
    
    public Square[,] gridArray;


    public Grid(int width, int height)
    {
        this.width = width;
        this.height = height;
// cellSize vaut 1 pour correspondre aux tailles des mesh des tuiles
        this.cellSize = 1; 

        gridArray = new Square[width, height];
// Dessin du quadrillage
        for (int x=0; x<gridArray.GetLength(0); x++){
            for (int y=0;y<gridArray.GetLength(1); y++){ 
                Debug.DrawLine(new Vector3(x,0.1f,y), new Vector3(x,0.1f,y+1), Color.white, 100f);
                Debug.DrawLine(new Vector3(x,0.1f,y), new Vector3(x+1,0.1f,y), Color.white, 100f);
            }
        }
    }
    
    Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x,y) * cellSize;
    }
}
