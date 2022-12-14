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

    public NetworkVariable<bool> isSelected = new NetworkVariable<bool>(false);

    private ulong selectingClient = 0;

    public GameManager gameManager;

    // private uint selectionToken = 1; // May be useful

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer){
            isSelected.OnValueChanged += Server_AnimateSelected;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    [ServerRpc(RequireOwnership = false)]
    public void MoveToServerRpc(int x, int y){
        if (grid == null) return;
        Vector3 newPosition = new Vector3(x+0.5f, 0, y+0.5f);
        transform.position = newPosition;
        this.x.Value = x;
        this.y.Value = y;
        ChangeCurrentPositionClientRpc();
    }

    [ClientRpc]
    private void ChangeCurrentPositionClientRpc(){
        this.currentPosition = grid.gridArray[x.Value,y.Value];
    }


    [ServerRpc(RequireOwnership=false)]
    public void ToggleSelectPawnServerRpc(bool selected, ServerRpcParams serverRpcParams = default){
        // Select if pawn was not selected already
        if (!isSelected.Value && selected){
            isSelected.Value = true;
            selectingClient = serverRpcParams.Receive.SenderClientId;
        }
        // Unselect if the sending client is the one which selected it
        else if (isSelected.Value && selectingClient == serverRpcParams.Receive.SenderClientId){
            isSelected.Value = selected;
        }
        else {
            return;
        }

        ClientRpcParams rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{serverRpcParams.Receive.SenderClientId}
            }
        };

        gameManager.TogglePawnSelectionClientRpc(selected, rpcParams);
    }

    private void Server_AnimateSelected(bool previous, bool selected){
        if (selected == previous) return;
        if (selected){
            transform.position += new Vector3(0,0.5f,0);
        }
        else {
            transform.position -= new Vector3(0,0.5f,0);
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
        //TODO
    }


    public void Explore()
    {
        //TODOs
    }


}
