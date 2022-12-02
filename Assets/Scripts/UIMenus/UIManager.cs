using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject escapeMenu;
    private void Awake() {
        escapeMenu.SetActive(false);
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

    public void DisplayEscapeMenu(){
        escapeMenu.SetActive(true);
    }

    public void HideEscapeMenu(){
        escapeMenu.SetActive(false);
    }

}
