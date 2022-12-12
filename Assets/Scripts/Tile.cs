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
    public string meshName;
    public Square[,] squares; //Toutes les cases qui sont dessus
    public bool isFirstTile; //Vrai si c'est la tuile centre commercial de depart
    public Direction.Direction orientation; // Correspond a l'orientation de la fleche indiquee sur chaque tuile; vaut null si c'est la firstTile

    public Tile(bool isFirstTile, Square.squareType[,] types, string meshName)
    {
        this.isFirstTile = isFirstTile;
        this.orientation = Direction.Direction.NORTH;
        // Permet de relier la tuile avec son avatar par le nom du mesh. Utile pour certaines choses
        this.meshName = meshName;

        Square[,] squares = new Square[4,4];
        for (int i=0; i<4;i++){
            for (int j=0; j<4; j++){
               squares[i,j]=new Square (types[i,j]); 
            }
        }
        this.squares = squares;
        for (int i=0; i<4; i++){
            this.squares[i,3].up = new Square(Square.squareType.NoGo);
            this.squares[i,0].down = new Square(Square.squareType.NoGo);

            this.squares[0,i].left = new Square(Square.squareType.NoGo);
            this.squares[3,i].right = new Square(Square.squareType.NoGo);
        }
        

    }

    public Tile(bool isFirstTile, Square[,] squares, string meshName)
    {
        this.isFirstTile = isFirstTile;
        this.squares = squares;
        this.meshName = meshName;
    }

    public Square GetSquare(int x, int y)
    {
        if (x>=0 && x<4)
        {
            return this.squares[x,y];
        }
        else {return null;}
    }

    private Tile rotateTile(){
        Square[,] newSquares = new Square [4,4];
        for (int i=0;i<4;i++){
            for (int j=0;j<4;j++){
                newSquares[i,j] = new Square (this.squares[3-j,i].type);
                newSquares[i,j].up = this.squares[i,j].left;
                newSquares[i,j].left = this.squares[i,j].down;
                newSquares[i,j].down = this.squares[i,j].right;
                newSquares[i,j].right = this.squares[i,j].up;
            }
        }
        Tile newTile = new Tile (false, newSquares, this.meshName);
        return newTile;

    }

    private void setBasicNeighbours(){
        for (int i=0; i<4; i++){
            this.squares[i,3].up = new Square(Square.squareType.NoGo);
            this.squares[i,3].down = this.squares[i,2];
            this.squares[i,2].up = this.squares[i,3];
            this.squares[i,2].down = this.squares[i,1];
            this.squares[i,1].up = this.squares[i,2];
            this.squares[i,1].down = this.squares[i,0];
            this.squares[i,0].up = this.squares[i,1];
            this.squares[i,0].down = new Square(Square.squareType.NoGo);

            this.squares[0,i].left = new Square(Square.squareType.NoGo);
            this.squares[0,i].right = this.squares[1,i];
            this.squares[1,i].left = this.squares[0,i];
            this.squares[1,i].right = this.squares[2,i];
            this.squares[2,i].left = this.squares[1,i];
            this.squares[2,i].right = this.squares[3,i];
            this.squares[3,i].left = this.squares[2,i];
            this.squares[3,i].right = new Square(Square.squareType.NoGo);
        }
    }

    private void addwall(int x1, int y1, int x2, int y2){
        if (x1==x2){
            if (y1<y2){
                this.squares[x1,y1].up = new Square(Square.squareType.NoGo);
                this.squares[x2,y2].down = new Square(Square.squareType.NoGo);
            }
            else {
                this.squares[x1,y1].down = new Square(Square.squareType.NoGo);
                this.squares[x2,y2].up = new Square(Square.squareType.NoGo);
            }
        }
        if (y1==y2){
            if (x1<x2){
                this.squares[x1,y1].right = new Square(Square.squareType.NoGo);
                this.squares[x2,y2].left = new Square(Square.squareType.NoGo);
            }
            else {
                this.squares[x1,y1].left = new Square(Square.squareType.NoGo);
                this.squares[x2,y2].right = new Square(Square.squareType.NoGo);
            }
        }
    }
}
