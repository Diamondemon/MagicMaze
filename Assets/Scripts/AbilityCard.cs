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

    public List<action> actions;

    public AbilityCard(bool moveRight=false, bool moveLeft=false, bool moveUp = false, bool moveDown=false)
    {
        this.actions = new List<action>();

        Debug.Log("ON CREE UNE NOUVELLE ABILITY CARD");
        if(moveRight)
            this.AddAction(action.moveRight);

        if(moveLeft)
            this.AddAction(action.moveLeft);

        if(moveUp)
            this.AddAction(action.moveUp);

        if(moveDown)
            this.AddAction(action.moveDown);

    }

    private void AddAction(action a)
    {
        this.actions.Add(a);
    }

    public action[] GetActions(){ return this.actions.ToArray(); }

    public action GetAction(int i)
    {
        return actions[i];
    }

    //Affiche toutes les actions qu'on peut faire avec le pion selectionne
    public void ShowPossibleAction(PawnController pawn)
    {
        Square pawPos = pawn.currentPosition;

        foreach (action a in actions)
        {
            if(a == action.escalator || a == action.explore || a == action.teleport)
            {
                //On est pas cense passer par la, vu qu'on utilise pas ces actions 
            } else {

                // On se contente des cas ou les actions sont "bouger dans une des 4 directions"
                // soit moveLeft, moveRight, moveUp, moveDown
                Square nextSquare = GetNextSquareByAction(a, pawPos);
                while(nextSquare.type != Square.squareType.NoGo)
                {
                    //Higlights Square;

                    //AddOnClick("Move");

                    nextSquare = GetNextSquareByAction(a, nextSquare);
                }


            }
        }
    }


    // Prend en entree une square et une action (suppose un moveLeft moveRight etc.), et renvoie la case voisine a cette direction
    Square GetNextSquareByAction(action a, Square s){
        switch(a)
        {
            case action.moveDown:
                return s.down;
            case action.moveLeft:
                return s.left;
            case action.moveUp:
                return s.up;
            case action.moveRight:
                return s.right;

            default:
                Debug.Log("error : action invalide dans GetNextSquareByAction");
                return null;
        }
    }


    void DoAction(action a, PawnController c, Square s = null)
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
