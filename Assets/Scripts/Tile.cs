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

public class Tile : MonoBehaviour
{
    
    public Square[] squares; //Toutes les cases qui sont dessus

    public bool isFirstTile; //Vrai si c'est la tuile centre commercial de d�part

    private Direction.Direction orientation; // Correspond � l'orientation de la fl�che indiqu�e sur chaque tuile; vaut null si c'est la firstTile

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }




}
