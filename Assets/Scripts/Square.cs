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

    public Grid grid;

    public Square escalator; //la Square de l'autre cot� de l'escalator, vaut null sinon
    public squareType type;
    public bool isHighighted = false;
    
    public Square () {
        this.type = squareType.Normal;
        this.escalator = null;
        this.grid = null;
    }

    public Square (squareType type, Grid grid, Square escalator=null)
    {
        this.type = type;
        this.escalator = escalator;
        this.grid = grid;
    }

    public Square GetEscalator() { return this.escalator; }

    public Vector3 Get3DPosition()
    {
        //TODO
        throw new NotImplementedException();
    }
}
