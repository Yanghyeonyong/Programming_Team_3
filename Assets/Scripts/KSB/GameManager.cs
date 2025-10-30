using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int Score;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[GM] Awake set Instance");
    }

    public void Load(int buildIndex) => SceneManager.LoadScene(buildIndex);
}
