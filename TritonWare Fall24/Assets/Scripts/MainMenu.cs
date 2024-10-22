using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown DifficultySelector;

    private void Awake()
    {
        SceneController.Instance.SelectDifficulty(1);
    }

    public void StartGame()
    {
        SceneController.Instance.StartGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetDifficulty()
    {
        SceneController.Instance.SelectDifficulty(DifficultySelector.value);
    }
}