using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{

    [SerializeField] private AudioClip _titleMusic;
    private AudioSource _audioSource;

    //최정욱 추가
    public float _titleMusicProgress = 0f;
    public int _selectedPlayerIndex;
    public AsyncOperation _preloadOperation;
    public void StartPreloadingGameScene()
    {
        Application.backgroundLoadingPriority = ThreadPriority.High;
        // Start preloading the game scene asynchronously
        _preloadOperation = SceneManager.LoadSceneAsync(1);
        _preloadOperation.allowSceneActivation = false;
    }

    public void ActivatePreloadedGameScene()
    {

        if (_preloadOperation != null)
        {
            _preloadOperation.allowSceneActivation = true;
        }
        Application.backgroundLoadingPriority = ThreadPriority.Normal;
    }

    public void SelectFemalePlayer()
    {
        _selectedPlayerIndex = 0;
    }
    public void SelectMalePlayer()
    {
        _selectedPlayerIndex = 1;
    }
    public void LoadMenu()
    {
        LoadByIndex(0);
    }

    public void LoadGameScene()
    {
        _audioSource.Pause();
        _titleMusicProgress = _audioSource.time;    
        LoadByIndex(1);
    }
    public void LoadCreditScene()
    {
        LoadByIndex(2);
    }


    public static SceneChanger Instance { get; private set; }


    void OnTitleScreenLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0 && _titleMusic != null)
        {
            _audioSource.clip = _titleMusic;
            _audioSource.time = 0f;
            //_audioSource.loop = true;
            _audioSource.Play();
        }
    }

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

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _titleMusic;
        _audioSource.Play();

        SceneManager.sceneLoaded += OnTitleScreenLoad;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnTitleScreenLoad;
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
