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
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Time.timeScale = 1;
        if(UIManager.instance != null) UIManager.instance.gameIsPaused = false;
        if(ManaResource.instance != null) ManaResource.instance.allowTimeControl = true;
    }

    public void DoPanel(GameObject panel)
    {
        panel.SetActive(true);
        PanelSlideUI panelScript = panel.GetComponent<PanelSlideUI>();
        panelScript.SlideIn();
    }

    public void LoadSceneByIndex(GameObject panel = null)
    {
        if(panel != null)
            panel.GetComponent<PanelSlideUI>().SlideOut();
        StartCoroutine(LoadSceneAsync("buh", true));
    }

    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    private IEnumerator LoadSceneAsync(string sceneName, bool byIndex = false)
    {
        if (byIndex)
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            AsyncOperation operation = SceneManager.LoadSceneAsync(currentIndex + 1);
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
