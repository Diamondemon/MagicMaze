using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;

public class GameUI : NetworkBehaviour 
{
    [SerializeField] Button quitButton;

    void Awake(){
        quitButton.onClick.AddListener(() =>{
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("MainMenu");
        }
        );
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
