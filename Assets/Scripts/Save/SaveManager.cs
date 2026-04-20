using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("game saved to: " + SavePath);
    }
    public static SaveData LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("no save file found, creating default save at " + SavePath);
            return CreateDefaultSave();
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null)
        {
            Debug.LogWarning("save file corrupted, creating default save");
            return CreateDefaultSave();
        }

        return data;
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("save file deleted");
        }
    }
    private static SaveData CreateDefaultSave()
    {
        SaveData data = new SaveData();

        int spellCount = 4;

        data.gloveUnlocked = new bool[spellCount];
        data.mantleUnlocked = new bool[spellCount];
        data.hatUnlocked = new bool[spellCount];
        data.bootUnlocked = new bool[spellCount];

        data.gloveUnlocked[0] = true;
        data.mantleUnlocked[0] = true;
        data.hatUnlocked[0] = true;
        data.bootUnlocked[0] = true;

        data.firstTimeExitGate = true;

        data.currentGloveSpell = 0;
        data.currentMantleSpell = 0;
        data.currentHatSpell = 0;
        data.currentBootSpell = 0;

        return data;
    }
}