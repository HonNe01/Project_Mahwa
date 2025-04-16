using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public GameObject pauseMenu;


    public bool isBattle;
    public bool isPause = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Update()
    {
        // 메인화면이 아닐때만
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (isPause)
            {
                GameResume();
            }
            else
            {
                GamePause();
            }
            
        }
    }

    public void GamePause()
    {
        Time.timeScale = 0f;
        isPause = true;

        pauseMenu?.SetActive(true);
    }

    public void GameResume()
    {
        Time.timeScale = 1f;
        isPause = false;


        pauseMenu?.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        isPause = false;

        SceneManager.LoadScene(0);
    }


    public void GameOver()
    {

    }
}
