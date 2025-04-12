using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject optionPanel;

    // utilise un enum pour les scenes
    [Header("Scenes")]
    [SerializeField] private string selectLevelSceneName = "SelectLevelScene";

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenOption()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(true);
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(selectLevelSceneName);
    }

    public void CloseOption()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
        }
    }
}
