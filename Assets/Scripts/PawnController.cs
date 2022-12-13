using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Direction;

public class PawnController : NetworkBehaviour 
{

    public enum Color
    {
        yellow, red, green, blue
    }

    public NetworkVariable<int> x;
    public NetworkVariable<int> y;
    [SerializeField] public Color color;
    public Square currentPosition;

    public Grid grid;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.L)){
            testMoveServerRpc();
        }
    }

    [ServerRpc]
    public void testMoveServerRpc(){
        transform.position += Vector3.forward * 3* Time.deltaTime;
    }

    [ServerRpc(RequireOwnership = false)]
    public void moveToServerRpc(int x, int y){
        if (grid == null) return;
        Vector3 newPosition = new Vector3(x+0.5f, 0, y+0.5f);
        transform.position = newPosition;
        this.x.Value = x;
        this.y.Value = y;
        changeCurrenPositionClientRpc();
    }

    [ClientRpc]
    public void changeCurrenPositionClientRpc(){
        this.currentPosition = grid.gridArray[x.Value,y.Value];
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
