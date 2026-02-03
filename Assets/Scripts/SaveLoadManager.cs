using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameSaveData
{
    public int score;
    public int moves;
    public int rows;
    public int columns;
    public float elapsedTime;
    public List<int> matchedCardIds = new List<int>();
}

public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager instance;
    private string savePath;

    public static SaveLoadManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveLoadManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("SaveLoadManager");
                    instance = obj.AddComponent<SaveLoadManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        savePath = Application.persistentDataPath + "/gamedata.json";
    }

    public void SaveGame(int score, int moves, int rows, int columns, float elapsedTime, List<int> matchedCardIds)
    {
        GameSaveData data = new GameSaveData
        {
            score = score,
            moves = moves,
            rows = rows,
            columns = columns,
            elapsedTime = elapsedTime,
            matchedCardIds = matchedCardIds
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved to {savePath}");
    }

    public bool TryLoadGame(out GameSaveData data)
    {
        data = null;

        if (!File.Exists(savePath))
        {
            Debug.Log("No save file found.");
            return false;
        }

        try
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<GameSaveData>(json);
            Debug.Log("Game loaded successfully.");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading game: {e.Message}");
            return false;
        }
    }

    public void DeleteSaveFile()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted.");
        }
    }

    public bool HasSaveFile()
    {
        return File.Exists(savePath);
    }

    public string GetSavePath()
    {
        return savePath;
    }
}
