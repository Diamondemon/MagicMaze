using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    private NetworkManager mgr;
    [SerializeField] Button hostButton;
    [SerializeField] Button clientButton;
    [SerializeField] TMP_InputField hostField;
    [SerializeField] TMP_InputField portField;


    private void Awake(){
        hostButton.onClick.AddListener(()=>{
            changeHost();
            if (NetworkManager.Singleton.StartHost())
                gameObject.SetActive(false);
        });
        clientButton.onClick.AddListener(()=>{
            changeHost();
            if (NetworkManager.Singleton.StartClient())
                gameObject.SetActive(false);
        });
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Changes the host adress
    void changeHost(){
        if (ushort.TryParse(hostField.text, out ushort port)){
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                hostField.text,
                port
            );
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = port;
        }
        else {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                hostField.text,
                (ushort)8000
            );
        }
    }
}
