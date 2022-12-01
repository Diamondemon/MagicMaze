using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] Button hostButton;
    [SerializeField] Button clientButton;
    [SerializeField] TMP_InputField hostField;
    [SerializeField] TMP_InputField portField;


    private void Awake(){
        hostButton.onClick.AddListener(()=>{
            this.changeHost();
            if (NetworkManager.Singleton.StartHost()){
                SceneManager.LoadScene("GameScene");
            }
        });
        clientButton.onClick.AddListener(()=>{
            this.changeHost();
            if (NetworkManager.Singleton.StartClient()){
                SceneManager.LoadScene("GameScene");
            }
        });
    }

    // Changes the host adress
    void changeHost(){
        if (ushort.TryParse(portField.text, out ushort port)){
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                hostField.text,
                port
            );
        }
        else {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                hostField.text,
                (ushort)8000
            );
        }
    }
}
