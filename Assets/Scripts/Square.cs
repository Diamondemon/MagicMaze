using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square
{

    public enum squareType
    {
        NoGo, Normal, OutOrange, OutGreen, OutPurple, OutYellow, Timer, TeleporterOrange, TeleporterGreen, TeleporterPurple, TeleporterYellow
    }

//  A PRENDRE EN COMPTE : Les Squares n'ont pas d'orientation definie. Le up/right/left/down depend de l'orientation de la Tile sur laquelle elle se trouve.
 /**   */
    public Square up;
    public Square down;
    public Square left;
    public Square right;

    public int x { get; private set; }
    public int y { get; private set; }

    public Grid grid;

    public Square escalator; //la Square de l'autre cot� de l'escalator, vaut null sinon
    public squareType type;
    public bool isHighighted = false;
    
    public Square () {
        this.type = squareType.Normal;
        this.escalator = null;
        this.grid = null;
    }

    public Square (squareType type, Grid grid, int x, int y, Square escalator=null)
    {

        this.type = type;
        this.escalator = escalator;
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public Square (squareType type, int x, int y, Square escalator=null)
    {
        Debug.LogWarning("Creation d'un Square avec son parametre grid null");
        this.type = type;
        this.escalator = escalator;
        this.grid = null;
        this.x = x;
        this.y = y;
    }

    public Square GetEscalator() { return this.escalator; }


    public Vector3 Get3DPosition()
    {
        return this.grid.GetWorldPosition(this.x, this.y);
    }
}
