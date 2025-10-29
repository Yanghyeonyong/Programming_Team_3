using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : Singleton<GameStateManager>
{
    [SerializeField] private List<int> _maxEnemyNumberPerStage = new List<int>();

    
    [SerializeField] private List<GameObject> _playerPrefab; //0 여전사 1 무사


    [SerializeField] private GameObject _defaultPlayerPosition;


    [SerializeField] private GameObject _nextStageRecognitionPlanePrefab; //tag "NextStageRecognition"
    [SerializeField] private List<GameObject> _stagePrefabs; //스테이지별 프리팹들
    [SerializeField] private List<AudioClip> audioBGMs;



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

    private GameObject _currentPlayer;

    //private List<int> _maxEnemyPerStage;
    private List<GameObject> _stagePool; //


    private int _currentEnemyCount = 0;

    private int _currentTrackNumber = 0;

    //private bool _isStageCleared = false;
    private int _currentStage = 0;
    private bool _isInNextStageRecognitionPlane = false;
    public bool IsInNextStageRecognitionPlane { get { return _isInNextStageRecognitionPlane; } set { _isInNextStageRecognitionPlane = value; } }



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
        _audioSource.clip = audioBGMs[trackNumber];
        _audioSource.Play();

    }
    //void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.tag == "NextStageRecognition")
    //    {
    //        _isInNextStageRecognitionPlane = true;
    //    }
    //}



    void CheckStageClear()
    {
        if (_currentEnemyCount == 0)
        {
            //_isStageCleared = true;
            _nextStageRecognitionPlanePrefab.SetActive(true);
            // Show some UI indication that the stage is cleared


            if (_isInNextStageRecognitionPlane)
            {
                _currentStage++;
                //disable UI indication
                LoadStage(_currentStage);
                //만약 음악 변경 필요하면 PlayTrack(++_currentTrackNumber);
                _isInNextStageRecognitionPlane = false;
                //_isStageCleared = false;
                _nextStageRecognitionPlanePrefab.SetActive(false);
                _currentEnemyCount = _maxEnemyNumberPerStage[_currentStage];

            }






        }
    }


    void LoadStage(int stageNumber)
    {
        _currentPlayer.transform.position = _defaultPlayerPosition.transform.position;

        //Load Stage Logic



    }


    void InitialStart()
    {
        //_currentEnemyCount = _maxEnemyNumberPerStage[_currentStage];

        //_isGamePlayerSceneLoaded = true;
        //audioBGMs[2] = Resources.Load<AudioClip>("Audio/BGM/StageBGM");

        _audioSource = GetComponent<AudioSource>();
        PlayTrack(_currentTrackNumber);
        //_audioSource.Play();

        //251028 추가
        _nextStageDetector = Instantiate(_nextStageRecognitionPlanePrefab, transform); //자식으로 둠
        _nextStageDetector.SetActive(false);

        foreach (var prefab in _stagePrefabs) //앞에서 부터
        {
            if (_stagePool == null)
            {
                _stagePool = new List<GameObject>();
            }
            GameObject temp = Instantiate(prefab, transform); //기본 위치 및 회전값, 자식으로 둠
            temp.SetActive(false);
            _stagePool.Add(temp);
        }

        _currentPlayer = Instantiate(_playerPrefab[_currentPlayerType], _defaultPlayerPosition.transform.position, _playerPrefab[_currentPlayerType].transform.rotation, transform);
        //_maxStageNumberPerStage[0] = 3;
        _currentEnemyCount = _maxEnemyNumberPerStage[_currentStage];
        //_isStageCleared = false; //재확인용 초기화
        LoadStage(_currentStage);
        _isInitialStart = false;

    }

    void SubsequentStart()
    {
        _isPlayerAlive = true;

        _currentStage = 0;
        _currentTrackNumber = 0;
        //_isGamePlayerSceneLoaded = true;
        _audioSource.clip = audioBGMs[_currentTrackNumber];
        _audioSource.Play();
        LoadStage(_currentStage);
        _currentPlayer.transform.position = _defaultPlayerPosition.transform.position;
        _currentPlayer.SetActive(true);

        //_hasReturnedToGamePlayerScene = false;
        //_audioSource.Play();

    }

    void OnGamePlaySceneLoad(Scene scene, LoadSceneMode mode)
    {
        //mode는 single로 default임
        if (scene.buildIndex == 1 && _isInitialStart == false) //게임 플레이 씬
        {
            SubsequentStart();
            //_isGamePlayerSceneLoaded = true;
        }
    }

    void OnGamePlaySceneDeload(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 1) //게임 플레이 씬
        {

            _currentPlayer.SetActive(false);
            foreach (var stage in _stagePool)
            {
                stage.SetActive(false);
            }

            _audioSource.Pause();


            //_isGamePlayerSceneLoaded = false;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        InitialStart();
        SceneManager.sceneLoaded += OnGamePlaySceneLoad;
        SceneManager.sceneLoaded += OnGamePlaySceneDeload;


    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnGamePlaySceneLoad;
        SceneManager.sceneLoaded -= OnGamePlaySceneDeload;
    }




    // Update is called once per frame
    void Update()
    {
        //InitialStart(); //첫 실행시 한 번만
        //SubsequentStart(); //다른 화면에서 돌아올때 한번만

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {

            CheckStageClear();

        }

        if (SceneManager.GetActiveScene().buildIndex ==1 && _isPlayerAlive == false)
        {
            //Handle Player Death
            _currentPlayer.SetActive(false);
            //Show Game Over UI
            //Option to Restart or Return to Title
        }






    }

}

