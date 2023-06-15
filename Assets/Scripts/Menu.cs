using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour{

    public GameObject[] menuUI;
    public GameObject[] instructionsUI;
    [SerializeField]
    private Text text;
    //Запуск игры
    public void ButtonStartGame()
    {
        SceneManager.LoadScene(1);
    }
    
    //Выход из игры
    public void ButtonExitGame()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

}
