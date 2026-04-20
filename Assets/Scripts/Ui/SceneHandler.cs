using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler instance;

    private void Awake()
    {
        instance = this;
    }

    public void DoPanel(GameObject panel)
    {
        panel.SetActive(true);
        PanelSlideUI panelScript = panel.GetComponent<PanelSlideUI>();
        panelScript.SlideIn();
    }

    public void LoadScene(string sceneName, bool byIndexA = false)
    {
        StartCoroutine(LoadSceneAsync(sceneName, byIndexA));
    }

    private IEnumerator LoadSceneAsync(string sceneName, bool byIndex = false)
    {
        if (byIndex)
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            AsyncOperation operation = SceneManager.LoadSceneAsync(currentIndex++);
            while (!operation.isDone)
            {
                yield return null;
            }
        }

        else
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            while (!operation.isDone)
            {
                yield return null;
            }
        }
        
    }

    public void QuitGame()
    {
        //GameDataManager.instance.SaveGame();
        Debug.Log("quitting game...");
        Application.Quit();
    }
}
