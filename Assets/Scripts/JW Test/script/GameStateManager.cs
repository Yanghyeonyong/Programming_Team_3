using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : Singleton<GameStateManager>
{
    [SerializeField] private List<int> _maxEnemyNumberPerStage = new List<int>();
    [SerializeField] private List<GameObject> _playerPrefab; // 0: 전사, 1: 마법사 등
    [SerializeField] private GameObject _defaultPlayerPosition;

    [SerializeField] private GameObject _nextStageRecognitionPlanePrefab; // tag "NextStageRecognition"
    [SerializeField] private List<GameObject> _stagePrefabs;
    [SerializeField] private List<AudioClip> audioBGMs;

    private bool _isInitialStart = true;
    private bool _isPlayerAlive = true;
    public bool IsPlayerAlive { get => _isPlayerAlive; set => _isPlayerAlive = value; }

    public List<int> MaxStageNumberPerStage => _maxEnemyNumberPerStage;
    public int CurrentEnemyCount { get => _currentEnemyCount; set => _currentEnemyCount = value; }

    // ===== EnemySpawner 등에서 구독할 공개 이벤트 =====
    public event Action OnStageChanged;     // 스테이지 변경
    public event Action OnEnemyDied;        // 적 사망
    public int CurrentStage => _currentStage;
    public List<int> GetMaxEnemiesList() => _maxEnemyNumberPerStage;

    private AudioSource _audioSource;
    private int _currentPlayerType;
    private GameObject _currentPlayer;
    private List<GameObject> _stagePool;

    private int _currentEnemyCount = 0;
    private int _currentTrackNumber = 0;
    private int _currentStage = 0;

    private bool _isInNextStageRecognitionPlane = false;
    public bool IsInNextStageRecognitionPlane { get => _isInNextStageRecognitionPlane; set => _isInNextStageRecognitionPlane = value; }

    private GameObject _nextStageDetector;

    // ===== Public API =====
    public void ReportEnemyDied()
    {
        _currentEnemyCount = Mathf.Max(0, _currentEnemyCount - 1);
        OnEnemyDied?.Invoke();
        CheckStageClear();
    }

    public void PlayTrack(int trackNumber)
    {
        _currentTrackNumber = trackNumber;
        if (audioBGMs != null && trackNumber >= 0 && trackNumber < audioBGMs.Count)
        {
            if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = audioBGMs[trackNumber];
            _audioSource.Play();
        }
    }

    // ===== Internals =====
    void CheckStageClear()
    {
        if (_currentEnemyCount != 0) return;

        if (_nextStageDetector != null) _nextStageDetector.SetActive(true); // UI 표시 등

        if (_isInNextStageRecognitionPlane)
        {
            _currentStage++;

            OnStageChanged?.Invoke();

            LoadStage(_currentStage);

            _isInNextStageRecognitionPlane = false;
            if (_nextStageDetector != null) _nextStageDetector.SetActive(false);

            if (_currentStage >= 0 && _currentStage < _maxEnemyNumberPerStage.Count)
                _currentEnemyCount = _maxEnemyNumberPerStage[_currentStage];
        }
    }

    void LoadStage(int stageNumber)
    {
        if (_currentPlayer != null && _defaultPlayerPosition != null)
            _currentPlayer.transform.position = _defaultPlayerPosition.transform.position;

        // TODO: stageNumber를 이용해 _stagePool 활성/비활성 관리
    }

    void InitialStart()
    {
        _audioSource = GetComponent<AudioSource>();
        PlayTrack(_currentTrackNumber);

        // 다음 스테이지 감지 오브젝트(비활성)
        if (_nextStageRecognitionPlanePrefab != null)
        {
            _nextStageDetector = Instantiate(_nextStageRecognitionPlanePrefab, transform);
            _nextStageDetector.SetActive(false);
        }

        // 스테이지 풀 준비
        _stagePool = new List<GameObject>();
        foreach (var prefab in _stagePrefabs)
        {
            var temp = Instantiate(prefab, transform);
            temp.SetActive(false);
            _stagePool.Add(temp);
        }

        // 플레이어 생성
        if (_playerPrefab != null && _playerPrefab.Count > 0)
        {
            _currentPlayer = Instantiate(
                _playerPrefab[_currentPlayerType],
                _defaultPlayerPosition != null ? _defaultPlayerPosition.transform.position : Vector3.zero,
                _playerPrefab[_currentPlayerType].transform.rotation,
                transform
            );
        }

        if (_currentStage >= 0 && _currentStage < _maxEnemyNumberPerStage.Count)
            _currentEnemyCount = _maxEnemyNumberPerStage[_currentStage];

        LoadStage(_currentStage);
        _isInitialStart = false;
    }

    void SubsequentStart()
    {
        _isPlayerAlive = true;
        _currentStage = 0;
        _currentTrackNumber = 0;

        if (audioBGMs != null && audioBGMs.Count > 0)
        {
            if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = audioBGMs[_currentTrackNumber];
            _audioSource.Play();
        }

        LoadStage(_currentStage);

        if (_currentPlayer != null && _defaultPlayerPosition != null)
        {
            _currentPlayer.transform.position = _defaultPlayerPosition.transform.position;
            _currentPlayer.SetActive(true);
        }
    }

    void OnGamePlaySceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1 && _isInitialStart == false)
        {
            SubsequentStart();
        }
    }

    void OnGamePlaySceneDeload(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 1)
        {
            if (_currentPlayer != null) _currentPlayer.SetActive(false);
            if (_stagePool != null) foreach (var stage in _stagePool) stage.SetActive(false);
            if (_audioSource != null) _audioSource.Pause();
        }
    }

    private void Start()
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

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            CheckStageClear();
        }

        if (SceneManager.GetActiveScene().buildIndex == 1 && _isPlayerAlive == false)
        {
            if (_currentPlayer != null) _currentPlayer.SetActive(false);
            // TODO: Game Over UI 처리 / 타이틀 이동 등
        }
    }
}
