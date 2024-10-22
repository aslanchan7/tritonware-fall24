using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
    public DifficultySetting SelectedDifficulty;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SelectDifficulty(1);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void SelectDifficulty(int level)
    {
        SelectedDifficulty = DifficultySetting.Difficulties[level];
    }


}
