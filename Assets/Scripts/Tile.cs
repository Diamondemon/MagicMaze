using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Direction
{
    public enum Direction
    {
        NORTH, EAST, SOUTH, WEST
    }
}

public class Tile
{
    
    private Square[,] squares; //Toutes les cases qui sont dessus
    private bool isFirstTile; //Vrai si c'est la tuile centre commercial de depart
    private Direction.Direction orientation; // Correspond a l'orientation de la fleche indiquee sur chaque tuile; vaut null si c'est la firstTile

    public Tile(bool isFirstTile, Direction.Direction orientation, Square[,] squares)
    {
        this.isFirstTile = isFirstTile;
        this.squares = new Square[4,4];

        if (this.isFirstTile)
        {
            this.orientation = Direction.Direction.NORTH;
        }
        else 
        {
            this.orientation = orientation;
        }
    }

    public Square GetSquare(int x, int y)
    {
        if (x>=0 && x<4 && y>=0 && y<4)
        {
            return this.squares[x,y];
        }
        else {return null;}
    }


}
