using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LobbyUI : NetworkBehaviour
{

    [SerializeField] Transform content;
    [SerializeField] GameObject playerProfilePrefab;
    [SerializeField] GameObject escapeMenu;
    [SerializeField] Button playButton;

    private bool escapePressed = false;
    private bool escapeMenuVisible = false;

    private const int MIN_PLAYERS_COUNT = 1;

    NetworkList<ulong> m_connectedClients;

    private void Awake() {
        escapeMenu.SetActive(false);
        m_connectedClients = new NetworkList<ulong>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsClient){
            m_connectedClients.OnListChanged += Client_ChangePlayerProfiles;
            playButton.gameObject.SetActive(false);
        }
        if (IsHost)
        {
            playButton.interactable = false;
            m_connectedClients.OnListChanged += Host_VerifyPlayerCount;
            m_connectedClients.Add(NetworkManager.Singleton.LocalClientId);
            playButton.gameObject.SetActive(true);
        }
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += Server_AddPlayerToList;
            NetworkManager.Singleton.OnClientDisconnectCallback += Server_RemovePlayerFromList;
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)){
            if (!escapePressed) {
                escapePressed = true;

                if (escapeMenuVisible)HideEscapeMenu();
                else DisplayEscapeMenu();
            }
        }
        else if (escapePressed) escapePressed = false;

    }

    private void Server_AddPlayerToList(ulong clientId){
        m_connectedClients.Add(clientId);
    }

    private void Server_RemovePlayerFromList(ulong clientId){
        m_connectedClients.Remove(clientId);
    }

    private void Host_VerifyPlayerCount(NetworkListEvent<ulong> changeEvent){
        if (m_connectedClients.Count >= MIN_PLAYERS_COUNT){
            playButton.interactable = true;
        }
        else {
            playButton.interactable = false;
        }
    }

    private void Client_ChangePlayerProfiles(NetworkListEvent<ulong> changeEvent){

        foreach (Transform child in content){
            GameObject.Destroy(child.gameObject);
        }

        foreach (ulong id in m_connectedClients){
            GameObject profilePanel = Instantiate(playerProfilePrefab,content);
            TextMeshProUGUI text = profilePanel.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "Player "+id + (id == NetworkManager.Singleton.LocalClientId? " (you)":"");
        }
    }

    public override void OnDestroy() {
        if (NetworkManager.Singleton != null){
            if (IsServer) {
                NetworkManager.Singleton.OnClientConnectedCallback -= Server_AddPlayerToList;
                NetworkManager.Singleton.OnClientDisconnectCallback -= Server_RemovePlayerFromList;
            }
        }

        if (IsClient){
            m_connectedClients.OnListChanged -= Client_ChangePlayerProfiles;
        }
        
        base.OnDestroy();
        
    }

    public void DisplayEscapeMenu(){
        escapeMenu.SetActive(true);
        escapeMenuVisible = true;
    }

    public void HideEscapeMenu(){
        escapeMenu.SetActive(false);
        escapeMenuVisible = false;
    }

    public void QuitGame(){
        NetworkManager.Singleton.Shutdown();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void BackToMainMenu(){
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }

    public void LaunchGame(){
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
