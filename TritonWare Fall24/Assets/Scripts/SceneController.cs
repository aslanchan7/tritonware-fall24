using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }


}
