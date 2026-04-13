using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{

    private void Awake()
    {
        PlayerPrefs.SetFloat("difficultintensity", 0.5f);
    }
    // Update is called once per frame
    void Update()
    {
        string current = SceneManager.GetActiveScene().name;

        // Only save if the scene is NOT one of the excluded ones
        if (current != "EndScene" && current != "StartScene" && current != "LevelProgressScene")
        {
            PlayerPrefs.SetString("LastSavedScene", current);
        }
    }
    public void ReloadLastScene()
    {
        string sceneToLoad = PlayerPrefs.GetString("LastSavedScene");
        SceneManager.LoadScene(sceneToLoad);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("PCGTest");
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void Winscreen()
    {
        SceneManager.LoadScene("EndScene");
    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exiting game");
    }
}
