using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance { get; private set; }
    public SaveData CurrentSave { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        CurrentSave = SaveManager.LoadGame();
    }

    public void SaveGame()
    {
        SaveManager.SaveGame(CurrentSave);
    }

    public void ResetSave()
    {
        SaveManager.DeleteSave();
        CurrentSave = SaveManager.LoadGame();
    }
}