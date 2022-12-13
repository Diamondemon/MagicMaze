using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Direction;
using Unity.Netcode;

[Serializable]
public class AbilityCard : INetworkSerializable, System.IEquatable<AbilityCard>
{

    public enum Action
    {
        moveRight,
        moveLeft,
        moveUp,
        moveDown,
        escalator,
        teleport,
        explore,
    }

    public List<Action> actions;

    public AbilityCard(bool moveRight=false, bool moveLeft=false, bool moveUp = false, bool moveDown=false)
    {
        this.actions = new List<Action>();

        Debug.Log("ON CREE UNE NOUVELLE ABILITY CARD");
        if(moveRight)
            this.AddAction(Action.moveRight);

        if(moveLeft)
            this.AddAction(Action.moveLeft);

        if(moveUp)
            this.AddAction(Action.moveUp);

        if(moveDown)
            this.AddAction(Action.moveDown);

    }

    private void AddAction(Action a)
    {
        this.actions.Add(a);
    }

    public Action[] GetActions(){ return this.actions.ToArray(); }

    public Action GetAction(int i)
    {
        return actions[i];
    }

    //Affiche toutes les actions qu'on peut faire avec le pion selectionne
    public void ShowPossibleAction(PawnController pawn)
    {
        Square pawPos = pawn.currentPosition;

        foreach (Action a in actions)
        {
            if(a == Action.escalator || a == Action.explore || a == Action.teleport)
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
    Square GetNextSquareByAction(Action a, Square s){
        switch(a)
        {
            case Action.moveDown:
                return s.down;
            case Action.moveLeft:
                return s.left;
            case Action.moveUp:
                return s.up;
            case Action.moveRight:
                return s.right;

            default:
                Debug.Log("error : action invalide dans GetNextSquareByAction");
                return null;
        }
    }


    void DoAction(Action a, PawnController c, Square s = null)
    {
        switch(a)
        {
            case Action.moveDown:
                c.Move(Direction.Direction.SOUTH);
                break;
            
            case Action.moveLeft:
                c.Move(Direction.Direction.WEST);
                break;

            case Action.moveRight:
                c.Move(Direction.Direction.EAST);
                break;

            case Action.moveUp:
                c.Move(Direction.Direction.NORTH);
                break;

            case Action.teleport:
                c.Teleport(s);
                break;
            
            case Action.escalator:
                c.TakeEscalator();
                break;

            case Action.explore:
                c.Explore();
                break; 
        }
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int length = 0;
        Action[] array;

        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out length);
            array = new Action[length];
            for (int i=0; i<length; i++){
                reader.ReadValueSafe(out array[i]);
            }
            actions.AddRange(array);
        }
        else
        {
            array = actions.ToArray();
            length = array.Length;
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(length);
            foreach (Action a in actions){
                writer.WriteValueSafe(a);
            }
        }
    }

    public bool Equals(AbilityCard other)
    {
        return actions.TrueForAll((Action action)=>{return other.actions.Contains(action);});
    }
}
