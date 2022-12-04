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

    public Square escalator; //la Square de l'autre cot� de l'escalator, vaut null sinon
    public squareType type;
    
    public Square () {
        this.type = squareType.Normal;
        this.escalator = null;
    }

    public Square (squareType type, Square escalator=null)
    {
        this.type = type;
        this.escalator = escalator;
    }

    public Square GetEscalator() { return this.escalator; }

}
