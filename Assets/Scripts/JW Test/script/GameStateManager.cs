using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameStateManager : Singleton<GameStateManager>
{
    [SerializeField] private List<int> _maxEnemyNumberPerStage = new List<int>();

    
    [SerializeField] private List<GameObject> _playerPrefab; //0 ������ 1 ����


    [SerializeField] private GameObject _defaultPlayerPosition;


    [SerializeField] private GameObject _nextStageRecognitionPlanePrefab; //tag "NextStageRecognition"
    [SerializeField] private List<GameObject> _stagePrefabs; //���������� �����յ�
    [SerializeField] private List<AudioClip> audioBGMs;
    [SerializeField] private AudioClip _deathScreenMusic;




    //������ ��û ����
    public UnityEvent<int> OnStageChanged = new UnityEvent<int>();
    public UnityEvent OnEnemyDied = new UnityEvent();

    public UnityEvent<bool> OnPlayerDeath = new UnityEvent<bool>();
    public UnityEvent<bool> OnGameRestart = new UnityEvent<bool>();

    public int CurrentStage { get { return _currentStage; } }



    //251028 �߰�
    private GameObject _nextStageDetector;



    private bool _isInitialStart = true;
    //private bool _isTitleSceneLoaded = false;
    //private bool _isTitleMusicPlaying = false;
    //private bool _hasReturnedToGamePlayerScene = false;
    private bool _isPlayerAlive = true;


    public bool IsPlayerAlive { get { return _isPlayerAlive; } set { _isPlayerAlive = value; } }
    //1���������� �ε��� 0
    public List<int> MaxStageNumberPerStage { get { return _maxEnemyNumberPerStage; } }



    //�ܺο��� �� ����� ī��Ʈ ���ҿ�
    public int CurrentEnemyCount { get { return _currentEnemyCount; } set { _currentEnemyCount = value; } }

    
    //public bool IsStageCleared { get { return _isStageCleared; } }
    

    //private bool _isGamePaused = false;
    //private bool _isGamePlayerSceneLoaded = false;

    private AudioSource _audioSource;

    //private GameObject _playerType;
    private int _currentPlayerType;
    public int CurrentPlayerType { get { return _currentPlayerType; } set { _currentPlayerType = value; } }
    //scene manager�� currentPlayer type �����ؾ��ҵ�


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

    

    //tracknumber 0 Ÿ��Ʋ �뷡
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
        Debug.Log("���� ���� �� " + _currentEnemyCount);
        
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
                //���� ���� ���� �ʿ��ϸ� PlayTrack(++_currentTrackNumber);
                _isInNextStageRecognitionPlane = false;
                //_isStageCleared = false;
                _nextStageRecognitionPlanePrefab.SetActive(false);
                _currentEnemyCount = _maxEnemyNumberPerStage[_currentStage];
                _currentEnemyCount += (int)(_currentEnemyCount / _killsRequiredForElite);

                Debug.Log("�� �Ѿ�Դ� : " + _currentStage);

            }






        }
    }


    void LoadStage(int stageNumber)
    {
        OnStageChanged.Invoke(stageNumber);
        _currentPlayer.transform.position = _defaultPlayerPosition.transform.position;

        //Load Stage Logic



    }


    void InitialStart()
    {
        _currentPlayer = Instantiate(_playerPrefab[_currentPlayerType], _defaultPlayerPosition.transform.position, _playerPrefab[_currentPlayerType].transform.rotation);
        //_currentPlayer = FindObjectOfType<Player>().gameObject;
        //_currentEnemyCount = _maxEnemyNumberPerStage[_currentStage];

        //_isGamePlayerSceneLoaded = true;
        //audioBGMs[2] = Resources.Load<AudioClip>("Audio/BGM/StageBGM");

        _audioSource = GetComponent<AudioSource>();
        PlayTrack(_currentTrackNumber);
        //_audioSource.Play();

        //251028 �߰�
        _nextStageDetector = Instantiate(_nextStageRecognitionPlanePrefab, transform); //�ڽ����� ��
        _nextStageDetector.SetActive(false);

        foreach (var prefab in _stagePrefabs) //�տ��� ����
        {
            if (_stagePool == null)
            {
                _stagePool = new List<GameObject>();
            }
            GameObject temp = Instantiate(prefab, transform); //�⺻ ��ġ �� ȸ����, �ڽ����� ��
            temp.SetActive(false);
            _stagePool.Add(temp);
        }

        //_maxStageNumberPerStage[0] = 3;
        _currentEnemyCount = _maxEnemyNumberPerStage[_currentStage];

        //������ �߰� ����Ʈ���� �߰��ϴ� �뵵
        _currentEnemyCount += (int)(_currentEnemyCount / _killsRequiredForElite);
        //_isStageCleared = false; //��Ȯ�ο� �ʱ�ȭ
        LoadStage(_currentStage);
        _isInitialStart = false;

    }

    void SubsequentStart()
    {
        if (_currentPlayer == null)
        {
            _currentPlayer = Instantiate(_playerPrefab[_currentPlayerType], _defaultPlayerPosition.transform.position, _playerPrefab[_currentPlayerType].transform.rotation);
        }
        _isPlayerAlive = true;

        _currentStage = 0;
        _currentTrackNumber = 0;
        //_isGamePlayerSceneLoaded = true;
        _audioSource.clip = audioBGMs[_currentTrackNumber];
        //play from beginning
        _audioSource.time = 0f;
        _audioSource.Play();
        
        LoadStage(_currentStage);
        _currentPlayer.transform.position = _defaultPlayerPosition.transform.position;
        _currentPlayer.GetComponent<Player>().ResetPlayer();
        _currentPlayer.SetActive(true);

        //_hasReturnedToGamePlayerScene = false;
        //_audioSource.Play();

    }

    void OnGamePlaySceneLoad(Scene scene, LoadSceneMode mode)
    {
        //mode�� single�� default��
        if (scene.buildIndex == 1 && _isInitialStart == false) //���� �÷��� ��
        {
            SubsequentStart();
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
            //_isGamePlayerSceneLoaded = true;
        }
    }

    void OnGamePlaySceneDeload(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 1) //���� �÷��� ��
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
    //void Start()
    //{
    //    //base.Awake();


    //}

    private List<Canvas> _uis;

    //UserInterface�� �˾Ƽ� ������ ���⿡ ��´�
    public GameObject _userInterface;

    [SerializeField] private int _killsRequiredForElite = 5;
    private void Start()
    {
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

        _userInterface.SetActive(true);

        //_deathScreenMusicTrackNumber = audioBGMs.Count -1;
        InitialStart();
        SceneManager.sceneLoaded += OnGamePlaySceneLoad;
        SceneManager.sceneLoaded += OnGamePlaySceneDeload;


    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnGamePlaySceneLoad;
        SceneManager.sceneLoaded -= OnGamePlaySceneDeload;
    }



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

    //Player.cs���� �˸�
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
        //InitialStart(); //ù ����� �� ����
        //SubsequentStart(); //�ٸ� ȭ�鿡�� ���ƿö� �ѹ���

        if (SceneManager.GetActiveScene().buildIndex == 1 && _isPlayerAlive == true)
        {

            CheckStageClear();

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
            _userInterface.SetActive(true);
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
            //SceneManager.LoadScene(0); //Ÿ��Ʋ ȭ������
        }




    }

}

