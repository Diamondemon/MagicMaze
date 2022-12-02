using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetcodeManager : MonoBehaviour
{
    [SerializeField] GameObject nwManager;
    private void Awake() {
        if (NetworkManager.Singleton == null){
            Instantiate(nwManager);
        }
    }
}
