using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square
{

    public enum squareType
    {
        Normal, Timer, Teleporter
    }

//  A PRENDRE EN COMPTE : Les Squares n'ont pas d'orientation definie. Le up/right/left/down depend de l'orientation de la Tile sur laquelle elle se trouve.
 /**   public Square up;
    public Square down;
    public Square left;
    public Square right; */

    private Tile tile;
    private Square escalator; //la Square de l'autre cot� de l'escalator, vaut null sinon
    private squareType type;

    public Square (Tile tile, squareType type, Square escalator=null)
    {
        this.tile = tile;
        this.type = type;
        this.escalator = escalator;
    }

    public Tile GetTile() { return this.tile; }
    public Square GetEscalator() { return this.escalator; }

}
