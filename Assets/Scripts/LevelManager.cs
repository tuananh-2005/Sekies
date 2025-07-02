using UnityEngine;

[System.Serializable]
public class LevelData
{
    public GameObject prefab;
    public Vector3 spawnPosition;
}

public class LevelManager : MonoBehaviour
{
    public LevelData[] levels;
    public Transform parent;
    private int currentIndex = 0;
    private GameObject currentInstance;

    void Start()
    {
        LoadLevel(currentIndex);
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levels.Length) return;

        if (currentInstance != null)
            Destroy(currentInstance);

        currentInstance = Instantiate(levels[index].prefab, levels[index].spawnPosition, Quaternion.identity, parent);
        currentIndex = index;
        Time.timeScale = 1f;
    }

    // Hàm để lấy chỉ số level hiện tại
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    // Hàm để lấy độ dài mảng levels
    public int GetLevelsLength()
    {
        return levels.Length;
    }
}



