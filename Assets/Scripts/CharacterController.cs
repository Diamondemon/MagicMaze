using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Direction;

public class CharacterController : MonoBehaviour
{

    public enum Color
    {
        yellow, orange, green, purple
    }

    public Color color;
    public Square currentPosition;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // A REFAIRE 
    // TODO
    public void Move(Direction.Direction d)
    {
        switch (d)
        {
            case Direction.Direction.NORTH:
                if (currentPosition.up != null)
                {
                    currentPosition = currentPosition.up;
                }
                else 
                {
                    Debug.Log("Error : no square on the top");
                }
                break;

            case Direction.Direction.EAST:
                if (currentPosition.right != null)
                {
                    currentPosition = currentPosition.right;
                }
                else 
                {
                    Debug.Log("Error : no square on the right");
                }
                break;
            
            case Direction.Direction.SOUTH:
                if (currentPosition.down != null)
                {
                    currentPosition = currentPosition.down;
                }
                else 
                {
                    Debug.Log("Error : no square on the bot");
                }
                break;

            case Direction.Direction.WEST:
                if (currentPosition.left != null)
                {
                    currentPosition = currentPosition.left;
                }
                else 
                {
                    Debug.Log("Error : no square on the left");
                }
                break;


        }
    }

    public void TakeEscalator()
    {
        if (currentPosition.GetEscalator() != null)
        {
            currentPosition = currentPosition.GetEscalator();
        }
        else 
        {
            Debug.Log("Error : no escalator here");
        }
    }

    public void Teleport(Square s)
    {
        //TODO
    }


    public void Explore()
    {
        //TODOs
    }


}
