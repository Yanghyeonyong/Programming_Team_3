using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtons : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1); // Build Settings¿« Index 1 æ¿ ∑ŒµÂ
    }

    public void LoadMainGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene("Title");
    }
}

