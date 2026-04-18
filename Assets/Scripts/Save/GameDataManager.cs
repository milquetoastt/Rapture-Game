using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance { get; private set; }
    public SaveData CurrentSave { get; private set; }

    private void Awake()
    {
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

    public void OnApplicationQuit()
    {
        SaveGame();
    }
}