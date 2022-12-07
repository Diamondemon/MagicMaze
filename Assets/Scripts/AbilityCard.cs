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

    private action[] actions;

    public action[] GetActions(){ return this.actions; }

    public action GetAction(int i)
    {
        if ( i >= this.actions.Length)
        { 
            Console.Log("Error : wrong index to getAction")
            return null;
        }

        return action[i];
    }

    //Affiche toutes les actions qu'on peut faire avec le pion selectionne
    void ShowPossibleAction(CharacterController pawn, PlayerController player)
    {
        Square pawPos = pawn.currentPosition;

        foreach (action a in player.abilityCard.GetActions())
        {
            if(a == action.escalator || a == action.explore || a == action.teleport)
            {
                //On est pas cense passer par la, vu qu'on utilise pas ces actions 
            } else {

                // On se contente des cas ou les actions sont "bouger dans une des 4 directions"
                // soit moveLeft, moveRight, moveUp, moveDown



            }
        }
    }


    // Prend en entree une square
    Square GetNextSquareByAction(action a, Square s){
        switch(a)
        {
            case action.moveDown:
                return 
        }
    }


    void DoAction(action a, CharacterController c, Square s = null)
    {
        switch(a)
        {
            case action.moveDown:
                c.Move(Direction.Direction.SOUTH);
                break;
            
            case action.moveLeft:
                c.Move(Direction.Direction.WEST);
                break;

            case action.moveRight:
                c.Move(Direction.Direction.EAST);
                break;

            case action.moveUp:
                c.Move(Direction.Direction.NORTH);
                break;

            case action.teleport:
                c.Teleport(s);
                break;
            
            case action.escalator:
                c.TakeEscalator();
                break;

            case action.explore:
                c.Explore();
                break; 


        }
    }
    
}
