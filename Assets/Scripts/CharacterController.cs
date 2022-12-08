using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Direction;

public class CharacterController : NetworkBehaviour 
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
        if (Input.GetKey(KeyCode.L)){
            transform.position += Vector3.forward * 3* Time.deltaTime;
        }
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
        switch(color)
        {
            case Color.yellow:
            if (s.type == Square.squareType.TeleporterYellow)
            {
                this.currentPosition = s;
            } else { Debug.Log("Error : yellow character tried to teleport to a invalid position"); }
            break;

            case Color.green:
            if (s.type == Square.squareType.TeleporterGreen)
            {
                this.currentPosition = s;
            } else { Debug.Log("Error : green character tried to teleport to a invalid position"); }
            break;

            case Color.orange:
            if (s.type == Square.squareType.TeleporterOrange)
            {
                this.currentPosition = s;
            } else { Debug.Log("Error : orange character tried to teleport to a invalid position"); }
            break;

            case Color.purple:
            if (s.type == Square.squareType.TeleporterPurple)
            {
                this.currentPosition = s;
            } else { Debug.Log("Error : purple character tried to teleport to a invalid position"); }
            break;

        }
        
    }


    public void Explore()
    {
        //TODO
    }


}
