using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public void SetGrid4x4()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGridSize(4, 4);
        }
    }

    public void SetGrid3x4()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGridSize(3, 4);
        }
    }

    public void SetGrid2x4()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGridSize(2, 4);
        }
    }

    public void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
        }
    }

    public void SaveOrLoadGame()
    {
        if (SaveLoadManager.Instance == null || GameManager.Instance == null)
            return;

        if (SaveLoadManager.Instance.HasSaveFile())
        {
            if (SaveLoadManager.Instance.TryLoadGame(out GameSaveData data))
            {
                GameManager.Instance.SetGridSize(data.rows, data.columns);
            }
        }
        else
        {
            SaveLoadManager.Instance.SaveGame(
                GameManager.Instance.Score,
                GameManager.Instance.Moves,
                GameManager.Instance.Rows,
                GameManager.Instance.Columns,
                GameManager.Instance.ElapsedTime,
                GameManager.Instance.GetMatchedCardIds()
            );
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
