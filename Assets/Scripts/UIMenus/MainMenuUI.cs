using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainButtonsPanel;
    [SerializeField] private GameObject networkManagerPanel;
    [SerializeField] private GameObject optionsPanel;

    void Awake(){
        mainButtonsPanel.SetActive(true);
        networkManagerPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void QuitGame(){
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void DisplayNetworkMenu(){
        mainButtonsPanel.SetActive(false);
        networkManagerPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void DisplayMainMenu(){
        mainButtonsPanel.SetActive(true);
        networkManagerPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void DisplayOptionsMenu(){
        mainButtonsPanel.SetActive(false);
        networkManagerPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }


    //DEBUG FUNCTION
    public void DisplayGameScene(){
        SceneManager.LoadScene("GameScene");
    }
}
