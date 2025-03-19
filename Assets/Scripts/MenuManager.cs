using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene("DevScene");
    }
}
