using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameStateManager : Singleton<GameStateManager>
{
    [SerializeField] private List<int> _maxEnemyNumberPerStage = new List<int>();

    
    [SerializeField] private List<GameObject> _playerPrefab; //0 여전사 1 무사


    [SerializeField] private GameObject _defaultPlayerPosition;


    [SerializeField] private GameObject _nextStageRecognitionPlanePrefab; //tag "NextStageRecognition"
    [SerializeField] private List<GameObject> _stagePrefabs; //스테이지별 프리팹들
    [SerializeField] private List<AudioClip> audioBGMs;
    [SerializeField] private AudioClip _deathScreenMusic;



    public bool _isBossAlive = false;

    //수현님 요청 사항
    public UnityEvent<int> OnStageChanged = new UnityEvent<int>();
    public UnityEvent OnEnemyDied = new UnityEvent();

    public UnityEvent<bool> OnPlayerDeath = new UnityEvent<bool>();
    public UnityEvent<bool> OnGameRestart = new UnityEvent<bool>();

    public int CurrentStage { get { return _currentStage; } }



    //251028 추가
    private GameObject _nextStageDetector;



    private bool _isInitialStart = true;
    //private bool _isTitleSceneLoaded = false;
    //private bool _isTitleMusicPlaying = false;
    //private bool _hasReturnedToGamePlayerScene = false;
    private bool _isPlayerAlive = true;


    public bool IsPlayerAlive { get { return _isPlayerAlive; } set { _isPlayerAlive = value; } }
    //1스테이지가 인덱스 0
    public List<int> MaxStageNumberPerStage { get { return _maxEnemyNumberPerStage; } }



    //외부에서 적 사망시 카운트 감소용
    public int CurrentEnemyCount { get { return _currentEnemyCount; } set { _currentEnemyCount = value; } }

    
    //public bool IsStageCleared { get { return _isStageCleared; } }
    

    //private bool _isGamePaused = false;
    //private bool _isGamePlayerSceneLoaded = false;

    private AudioSource _audioSource;

    //private GameObject _playerType;
    private int _currentPlayerType;
    public int CurrentPlayerType { get { return _currentPlayerType; } set { _currentPlayerType = value; } }
    //scene manager에 currentPlayer type 보관해야할듯


    public GameObject _currentPlayer;

    //private List<int> _maxEnemyPerStage;
    private List<GameObject> _stagePool; //


    private int _currentEnemyCount = 0;

    private int _currentTrackNumber = 0;

    //private bool _isStageCleared = false;
    private int _currentStage = 0;
    private bool _isInNextStageRecognitionPlane = false;
    public bool IsInNextStageRecognitionPlane { get { return _isInNextStageRecognitionPlane; } set { _isInNextStageRecognitionPlane = value; } }

    private SceneChanger _sceneChanger;

    public void PauseUnpauseMusic()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Pause();
        }
        else if (!_audioSource.isPlaying)
        {
            _audioSource.UnPause();

        }


    }

    

    //tracknumber 0 타이틀 노래
    public void PlayTrack(int trackNumber)
    {

        _currentTrackNumber = trackNumber;
        if (audioBGMs != null)
        {
            _audioSource.clip = audioBGMs[trackNumber];
            _audioSource.Play();
        }

    }
    //void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.tag == "NextStageRecognition")
    //    {
    //        _isInNextStageRecognitionPlane = true;
    //    }
    //}



    public void CheckStageClear()
    {

        Debug.Log("Checking Stage Clear");
        Debug.Log(CurrentEnemyCount);
        if (_currentEnemyCount <= 0 && _isBossAlive == false)

        {
            //_isStageCleared = true;
            _nextStageDetector.SetActive(true);
            // Show some UI indication that the stage is cleared
            _goNext.SetActive(true);

            if (_isInNextStageRecognitionPlane)
            {
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    enemy.gameObject.SetActive(false); //모든 적 비활성화
                }
                Debug.Log("Stage Cleared, Loading Next Stage");
                _currentStage++;
                if (_currentStage >= _stagePool.Count)
                {


                    Debug.Log("All Stages Cleared! Returning to Title Screen.");
                    
                _isInNextStageRecognitionPlane = false;
                    SceneManager.LoadScene(2); //메뉴 화면으로
                    return;
                }

                //disable UI indication
                LoadStage(_currentStage);
                if (SceneChanger.Instance._audioSource.isPlaying)
                {
                    StopCoroutine(WaitForTitleMusicToEnd());
                    SceneChanger.Instance._audioSource.Stop();
                    //SceneChanger.Instance._isMusicPlaying = false;
                }
                PlayTrack(_currentStage);
                //만약 음악 변경 필요하면 PlayTrack(++_currentTrackNumber);
                _isInNextStageRecognitionPlane = false;
                //_isStageCleared = false;
                _nextStageDetector.SetActive(false);
                _currentEnemyCount = _maxEnemyNumberPerStage[_currentStage];

                _goNext.SetActive(false);

            }






        }
    }


    void LoadStage(int stageNumber)
    {

        if (stageNumber >= 1)
        {
            _stagePool[stageNumber - 1].SetActive(false);
        }
        OnStageChanged.Invoke(stageNumber);
        _stagePool[stageNumber].SetActive(true);

        _currentPlayer.transform.position = _defaultPlayerPosition.transform.position;

        //Load Stage Logic



    }

    public float _killsRequiredForElite;
    

    void InitialStart()
    {
        _currentPlayerType = SceneChanger.Instance._selectedPlayerIndex;
        
        
        _currentPlayer = Instantiate(_playerPrefab[_currentPlayerType], _defaultPlayerPosition.transform.position, _playerPrefab[_currentPlayerType].transform.rotation);
        Debug.Log("Initial Start - Player Instantiated");
        //_currentPlayer = FindObjectOfType<Player>().gameObject;
        //_currentEnemyCount = _maxEnemyNumberPerStage[_currentStage];

        //_isGamePlayerSceneLoaded = true;
        //audioBGMs[2] = Resources.Load<AudioClip>("Audio/BGM/StageBGM");

        _audioSource = GetComponent<AudioSource>();
        PlayTrack(_currentTrackNumber);
        //_audioSource.Play();

        //251028 추가
        

        
        //LoadStage(_currentStage);

        //_maxStageNumberPerStage[0] = 3;
        _currentEnemyCount = _maxEnemyNumberPerStage[_currentStage];

        //Debug.Log("초기 적 수");
        //Debug.Log(_currentEnemyCount);
        _currentEnemyCount +=(int)(_currentEnemyCount/_killsRequiredForElite); //엘리트 몹 추가 반영
        //Debug.Log("엘리트 몹 추가 후 적 수");
        //Debug.Log(_currentEnemyCount);


        //_isStageCleared = false; //재확인용 초기화
        LoadStage(_currentStage);
        _isInitialStart = false;

    }

    void SubsequentStart()
    {

        if (EnemySpawner.Instance != null)
        {
            _killsRequiredForElite = EnemySpawner.Instance._killsRequiredForElite;
            Debug.Log($"Kills Required For Elite: {_killsRequiredForElite}");
        }
        _uis = new List<Canvas>(FindObjectsOfType<Canvas>());

        if (_uis != null)
        {
            foreach (var UI in _uis)
            {
                if (UI.gameObject.name == "DeathUI")
                {
                    _deathScreenUI = UI.gameObject;
                    _deathScreenUI.SetActive(false);
                    break;
                }

                //else if (UI.gameObject.name == "UserInterface")
                //{
                //    _userInterface = UI.gameObject;
                //    _userInterface.SetActive(false);
                //}
                //_deathScreenUI = FindAnyObjectByType<Canvas>().gameObject; //DeathUI Canvas
                //_deathScreenUI.SetActive(false);
            }
        }
        _currentPlayerType = SceneChanger.Instance._selectedPlayerIndex;
        if (_currentPlayer == null)
        {
            _currentPlayer = Instantiate(_playerPrefab[_currentPlayerType], _defaultPlayerPosition.transform.position, _playerPrefab[_currentPlayerType].transform.rotation);
            _currentPlayer.GetComponent<MoveComponent>().StartCheckAndDodge();
            Debug.Log("Dash On");
            Debug.Log("Subsequent Start - Player Instantiated");

        }
        _isPlayerAlive = true;

        _currentStage = 0;
        _currentTrackNumber = 0;
        if (_audioSource == null)
        {
                       _audioSource = GetComponent<AudioSource>();
        }
        //_isGamePlayerSceneLoaded = true;

        
        StartCoroutine(WaitForTitleMusicToEnd());


        LoadStage(_currentStage);
        _currentEnemyCount = _maxEnemyNumberPerStage[_currentStage];
        Debug.Log($"적 수 {_currentEnemyCount}");
        _currentEnemyCount += (int)(_currentEnemyCount / _killsRequiredForElite);
        Debug.Log(_currentEnemyCount);
        //엘리트 몹 추가 반영
        //_currentPlayer.transform.position = _defaultPlayerPosition.transform.position;


        StartCoroutine(WaitForUI());
        //_hasReturnedToGamePlayerScene = false;
        //_audioSource.Play();

    }

    IEnumerator WaitForTitleMusicToEnd()
    {
        while (SceneChanger.Instance._audioSource.isPlaying)
        {
            yield return null;
        }
        _audioSource.clip = audioBGMs[_currentStage];
        //play from beginning
        //_audioSource.time = SceneChanger.Instance._titleMusicProgress;
        _audioSource.Play();
    }


    IEnumerator WaitForUI()
    {
        while (_userInterface == null || _userInterface.GetComponent<UserInterfaceGame>()._isReady == false)
        {
            yield return null;
        }
        _currentPlayer.GetComponent<Player>().ResetPlayer();
        _currentPlayer.SetActive(true);
    }

    void OnGamePlaySceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (SceneChanger.Instance != null)
        {
            _currentPlayerType = SceneChanger.Instance._selectedPlayerIndex;
        }

        //mode는 single로 default임
        if (scene.buildIndex == 1) //게임 플레이 씬
        {
            SubsequentStart();


            

            //_isGamePlayerSceneLoaded = true;
        }
    }

    void OnGamePlaySceneDeload(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 1) //게임 플레이 씬
        {
            if (_currentPlayer != null)
            {
                _currentPlayer.SetActive(false);
            }
            foreach (var stage in _stagePool)
            {
                if (stage != null)
                {
                    stage.SetActive(false);
                }
            }

            if (_audioSource != null)
            {
                _audioSource.Pause();
            }

            //_isGamePlayerSceneLoaded = false;
        }
    }


    // Start is called before the first frame update
    //void Start()
    //{
    //    //base.Awake();


    //}

    private List<Canvas> _uis;

    //UserInterface는 알아서 본인을 여기에 담는다
    public GameObject _userInterface;


    private void Start()
    {

        _currentEnemyCount = _maxEnemyNumberPerStage[_currentStage];
        
        _currentEnemyCount += (int)(_currentEnemyCount / _killsRequiredForElite);
        _sceneChanger = SceneChanger.Instance;

        foreach (var prefab in _stagePrefabs) //앞에서 부터
        {
            if (_stagePool == null)
            {
                _stagePool = new List<GameObject>();
            }
            GameObject temp = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation, transform); //기본 위치 및 회전값, 자식으로 둠
            temp.SetActive(false);
            _stagePool.Add(temp);
        }

        _nextStageDetector = Instantiate(_nextStageRecognitionPlanePrefab, _nextStageRecognitionPlanePrefab.transform.position, _nextStageRecognitionPlanePrefab.transform.rotation, transform); //자식으로 둠
        _nextStageDetector.SetActive(false);
        //_uis = new List<Canvas>(FindObjectsOfType<Canvas>());

        //if (_uis != null)
        //{
        //    foreach (var UI in _uis)
        //    {
        //        if (UI.gameObject.name == "DeathUI")
        //        {
        //            _deathScreenUI = UI.gameObject;
        //            _deathScreenUI.SetActive(false);
        //            break;
        //        }

        //        //else if (UI.gameObject.name == "UserInterface")
        //        //{
        //        //    _userInterface = UI.gameObject;
        //        //    _userInterface.SetActive(false);
        //        //}
        //        //_deathScreenUI = FindAnyObjectByType<Canvas>().gameObject; //DeathUI Canvas
        //        //_deathScreenUI.SetActive(false);
        //    }
        //}

        //_userInterface.SetActive(true);



        //_deathScreenMusicTrackNumber = audioBGMs.Count -1;
        //InitialStart();

        SceneManager.sceneLoaded += OnGamePlaySceneLoad;
        SceneManager.sceneLoaded += OnGamePlaySceneDeload;


    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnGamePlaySceneLoad;
        SceneManager.sceneLoaded -= OnGamePlaySceneDeload;
    }
    

    //void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnGamePlaySceneLoad;
    //    SceneManager.sceneLoaded += OnGamePlaySceneDeload;
    //}
    //private void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnGamePlaySceneLoad;
    //    SceneManager.sceneLoaded -= OnGamePlaySceneDeload;
    //}

    //private void OnDestroy()
    //{
    //    SceneManager.sceneLoaded -= OnGamePlaySceneLoad;
    //    SceneManager.sceneLoaded -= OnGamePlaySceneDeload;
    //}


    public GameObject _goNext;

    private void DisableUI()
    {
        if (_userInterface != null)
        {
            if (_userInterface.activeSelf)
            {
                _userInterface.SetActive(false);
            }
            else
            {
                _userInterface.SetActive(true);
            }
        }

    }

    //Player.cs에서 알림
    //void CheckPlayerHealth()
    //{
    //    if (_currentPlayer == null)
    //    {
    //        if (_currentPlayer.)
    //        _isPlayerAlive = false;
    //        OnPlayerDeath.Invoke(!_isPlayerAlive);
    //    }
    //}


    GameObject _deathScreenUI; //= FindAnyObjectByType<Canvas>().gameObject; //DeathUI Canvas
    //public void LetSpawnerKnowPlayerDied()
    //{
    //   OnPlayerDeath.Invoke(false); //false means dead for the spawner
    //}


    //private int _deathScreenMusicTrackNumber; //to avoid multiple invocations

    public void LoadDeathScreen()
    {
        _userInterface.SetActive(false);
        _audioSource.clip = _deathScreenMusic;
        _audioSource.Play();

        //debug

        _deathScreenUI.SetActive(true);

    }
    //private void DeloadDeathSCreen()
    //{
    //    GameObject _deathScreenUI = FindAnyObjectByType<Canvas>().gameObject; //DeathUI Canvas
    //    _deathScreenUI.SetActive(false);
    //}

    private bool _restartRequested = false;

    public void RequestGameRestart()
    {
        _restartRequested = true;
    }

    // Update is called once per frame
    void Update()
    {
        //InitialStart(); //첫 실행시 한 번만
        //SubsequentStart(); //다른 화면에서 돌아올때 한번만

        if (SceneManager.GetActiveScene().buildIndex == 1 && _isPlayerAlive == true)
        {

            //CheckStageClear();

        }

        else if (SceneManager.GetActiveScene().buildIndex ==1 && _isPlayerAlive == false)
        {
            //Handle Player Death
            _currentPlayer.SetActive(false);
            //Show Game Over UI
            //Option to Restart or Return to Title
        }

        if (_restartRequested)
        {

            _stagePool[_currentStage].SetActive(false);
            _userInterface.SetActive(true);
            _currentStage = 0;

            _restartRequested = false;
            PauseUnpauseMusic();
            OnGameRestart.Invoke(true); //true means set Spawner Active

            SubsequentStart();
        }


        if (Input.GetKeyDown(KeyCode.Backspace) && IsPlayerAlive == true)
        {
            DisableUI();
            foreach (Transform child in _currentPlayer.transform)
            {
                if (child.name =="GreenArrow")
                {
                    if (child.gameObject.activeSelf)
                    {
                        child.gameObject.SetActive(false);
                    }
                    else
                    {
                        child.gameObject.SetActive(true);

                    }
                }
            

            }
            //SceneManager.LoadScene(0); //타이틀 화면으로
        }




    }

}

