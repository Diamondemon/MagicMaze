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
    
    public Square[,] squares; //Toutes les cases qui sont dessus
    public bool isFirstTile; //Vrai si c'est la tuile centre commercial de depart
    public Direction.Direction orientation; // Correspond a l'orientation de la fleche indiquee sur chaque tuile; vaut null si c'est la firstTile

    public Tile(bool isFirstTile, Square.squareType[,] types)
    {
        this.isFirstTile = isFirstTile;
        this.orientation = Direction.Direction.NORTH;

        Square[,] squares = new Square[4,4];
        for (int i=0; i<4;i++){
            for (int j=0; j<4; j++){
               squares[i,j]=new Square (types[i,j]); 
            }
        }
        this.squares = squares;

    }

    public Tile(bool isFirstTile, Direction.Direction orientation, Square[,] squares)
    {
        this.isFirstTile = isFirstTile;
        this.squares = squares;

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
        if (x>=0 && x<4)
        {
            return this.squares[x,y];
        }
        else {return null;}
    }


}
