using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene(1);
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
