using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{

    public static SceneChanger Instance { get; private set; }

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void LoadByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        SceneManager.LoadScene(sceneName);

    }


    public void LoadByIndex(int sceneIndex)
    {
        int count = SceneManager.sceneCountInBuildSettings;
        if (sceneIndex < 0 || sceneIndex >= count) return;
        SceneManager.LoadScene(sceneIndex);
    }


    public void LoadNext()
    {
        int count = SceneManager.sceneCountInBuildSettings;
        if (count == 0) return;

        int cur = SceneManager.GetActiveScene().buildIndex;
        int next = (cur < count - 1) ? cur + 1 : 0;
        SceneManager.LoadScene(next);
    }


    public void LoadPrev()
    {
        int count = SceneManager.sceneCountInBuildSettings;
        if (count == 0) return;

        int cur = SceneManager.GetActiveScene().buildIndex;
        int prev = (cur > 0) ? cur - 1 : count - 1;
        SceneManager.LoadScene(prev);
    }


    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
