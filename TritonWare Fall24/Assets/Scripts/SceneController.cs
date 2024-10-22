using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
    public DifficultySetting SelectedDifficulty;

    [Header("Transition")]
    public Animator fadeScreen;
    [SerializeField] float transitionTime = 1f;
    private int lastSceneIndex = -1;

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
        StartCoroutine(LoadLevel("Main"));
    }

    void Update()
    {
        if (lastSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            fadeScreen.Play("FadeScreen_End");
            lastSceneIndex = SceneManager.GetActiveScene().buildIndex;
        }
    }

    public void ReturnToMenu()
    {
        StartCoroutine(LoadLevel("Menu"));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        fadeScreen.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator PlayTransitionAnim()
    {
        fadeScreen.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
    }

    public void SelectDifficulty(int level)
    {
        SelectedDifficulty = DifficultySetting.Difficulties[level];
    }
}
