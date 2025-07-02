using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject togglePanel; 

    public void PlayLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void AboutMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleAssignedPanel()
    {
        if (togglePanel != null)
            togglePanel.SetActive(!togglePanel.activeSelf);
    }
}

