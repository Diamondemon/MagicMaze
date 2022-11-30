using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Direction;

public class AbilityCard : MonoBehaviour
{

    public enum action
    {
        moveRight,
        moveLeft,
        moveUp,
        moveDown,
        escalator,
        teleport,
        explore
    }

    public action[] actions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DoAction(action a, CharacterController c, Square s = null)
    {
        switch(a)
        {
            case moveDown:
                c.Move(Direction.Direction.SOUTH);
                break;
            
            case moveLeft:
                c.Move(Direction.Direction.WEST);
                break;

            case moveRight:
                c.Move(Direction.Direction.EAST);
                break;

            case moveUp:
                c.Move(Direction.Direction.NORTH);
                break;

            case teleport:
                c.Teleport(s);
                break;
            
            case escalator:
                c.TakeEscalator();
                break;

            case explore:
                c.Explore();
                break; 


        }
    }
    
}
