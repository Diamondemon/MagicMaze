using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyUI : NetworkBehaviour
{

    [SerializeField] Transform content;
    [SerializeField] GameObject playerProfilePrefab;
    [SerializeField] GameObject escapeMenu;

    private bool escapePressed = false;
    private bool escapeMenuVisible = false;

    private void Awake() {
        escapeMenu.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += testThat;
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

        if (Input.GetKey(KeyCode.T)){
            SpawnPlayerProfileClientRpc();
        }
    }

    private void testThat(ulong clientId){
        SpawnPlayerProfileClientRpc();
    }

    [ClientRpc]
    private void SpawnPlayerProfileClientRpc(){

            Debug.Log("I am called once "+
        NetworkManager.Singleton.LocalClientId);
        foreach (Transform child in content){
            GameObject.Destroy(child.gameObject);
        }
        foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds){
            Debug.Log("I am called aah ");
            GameObject profilePanel = Instantiate(playerProfilePrefab,content);
            TextMeshProUGUI text = profilePanel.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "Player "+id + (id == NetworkManager.Singleton.LocalClientId? " (you)":"");
        }
    }

    public override void OnDestroy() {
        base.OnDestroy();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= testThat;
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
}
