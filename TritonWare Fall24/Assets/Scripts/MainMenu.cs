using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneController.Instance.StartGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}