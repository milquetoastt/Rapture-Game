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

        instance = this;
        Time.timeScale = 1;
        if(UIManager.instance != null) UIManager.instance.gameIsPaused = false;
        if(ManaResource.instance != null) ManaResource.instance.allowTimeControl = true;
    }

    public void TurnOffFirstTimeGate()
    {
        GameDataManager.instance.TurnOffFirstTimeGate();
    }
    
    public void DoPanel(GameObject panel)
    {
        panel.SetActive(true);
        PanelSlideUI panelScript = panel.GetComponent<PanelSlideUI>();
        panelScript.SlideIn();
    }

    public void LoadSceneByIndex(GameObject panel = null, int currentDungeon = -1)
    {
        if(panel != null)
            panel.GetComponent<PanelSlideUI>().SlideOut();
        StartCoroutine(LoadSceneAsync("buh", true, currentDungeon));
    }

    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    public void ReturnToMainMenu()
    {
        if (PlayerController.instance != null)
            Destroy(PlayerController.instance.gameObject);

        if (UIManager.instance != null)
            Destroy(UIManager.instance.gameObject);

        if (ManaResource.instance != null)
            Destroy(ManaResource.instance.gameObject);

        if (GameDataManager.instance != null)
            Destroy(GameDataManager.instance.gameObject);

        LoadSceneByName("MainMenu");
    }
    private IEnumerator LoadSceneAsync(string sceneName, bool byIndex = false, int currentDungeon = -1)
    {
        if (byIndex)
        {
            int currentIndex = -1;

            if (currentDungeon != -1)
            {
                currentIndex = currentDungeon;
            }
            else if (PlayerController.instance != null)
            {
                currentIndex = PlayerController.instance.currentDungeonLayer;
            }
            else
            {
                currentIndex = SceneManager.GetActiveScene().buildIndex;
            }

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