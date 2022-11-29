using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{

    public enum squareType
    {
        Normal, Timer, Teleporter
    }

    Tile tile;

    //  A PRENDRE EN COMPTE : Les Squares n'ont pas d'orientation définie. Le up/right/left/down dépend de l'orientation de la Tile sur laquelle elle se trouve.
    public Square up;
    public Square down;
    public Square left;
    public Square right;

    public Square escalator; //la Square de l'autre coté de l'escalator, vaut null sinon

    public squareType type;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
