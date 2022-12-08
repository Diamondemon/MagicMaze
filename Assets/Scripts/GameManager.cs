using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiMgr;
    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_EDITOR
        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsConnectedClient){
            NetworkManager.Singleton.StartHost();
        }
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)){
            uiMgr.DisplayEscapeMenu();
        }
    }
}
