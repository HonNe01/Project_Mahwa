using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene("DevScene");
    }

    public void GameExit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GameEnd()
    {
        Application.Quit();
    }
}
