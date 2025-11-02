using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Spawn Settings")]
    [SerializeField] public int _killsRequiredForElite = 5;
    [SerializeField] private float _spawnDelay = 2f;
    [SerializeField] private bool _isActive = false;

    [SerializeField] private List<Transform> _spawnPoints;


    private int _spawnEnemiesCount = 0;

    private bool _isSpawning = false;
    private bool _isEliteSpawned = false;
    private int _aliveEnemies = 0;
    private int _totalKillsInStage = 0;

    private List<int> _maxEnemiesByStage = new List<int>();
    //private List<Transform> _spawnPoints = new List<Transform>();
    private GameStateManager _gameState;

    private Coroutine _spawnCoroutine = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Transform child in transform)
            _spawnPoints.Add(child);
    }

    private void Start()
    {
        _gameState = GameStateManager.Instance;
        if (_gameState == null) return;

        _gameState.OnPlayerDeath.AddListener(SetActive);
        _gameState.OnGameRestart.AddListener(SetActive);



        _gameState.OnStageChanged.AddListener(OnStageChanged);
        _gameState.OnEnemyDied.AddListener(OnEnemyDied);

        _maxEnemiesByStage = _gameState.MaxStageNumberPerStage != null ?
                             _gameState.MaxStageNumberPerStage :
                             new List<int>();

        _isActive = true;
        StartSpawningForStage(_gameState.CurrentStage);
    }

    private void OnDestroy()
    {
        if (_gameState != null)
        {
            _gameState.OnStageChanged.RemoveListener(OnStageChanged);
            _gameState.OnEnemyDied.RemoveListener(OnEnemyDied);
            _gameState.OnPlayerDeath.RemoveListener(SetActive);
            _gameState.OnGameRestart.RemoveListener(SetActive);
        }

        StopSpawnCoroutine();
    }

    private void OnStageChanged(int stageIndex)
    {
        _aliveEnemies = 0;
        _totalKillsInStage = 0;
        _enemyKillsToCheckEliteSpawn = 0;
        _isEliteSpawned = false;

        _spawnEnemiesCount = 0;

        StartSpawningForStage(stageIndex);
    }

    private int _enemyKillsToCheckEliteSpawn = 0;


    public void OnEnemyDied()
    {
        _aliveEnemies = Mathf.Max(0, _aliveEnemies - 1);
        _totalKillsInStage++;

        _enemyKillsToCheckEliteSpawn++;

        int maxEnemies = GetMaxEnemiesForStage(_gameState.CurrentStage) + (int)(GetMaxEnemiesForStage(_gameState.CurrentStage) / _killsRequiredForElite);

        // 엘리트 적 스폰
        //if (!_isEliteSpawned && _totalKillsInStage >= _killsRequiredForElite)
        //{
        //    _isEliteSpawned = true;
        //    SpawnEliteEnemy();
        //}
        if (_totalKillsInStage >= _killsRequiredForElite && _enemyKillsToCheckEliteSpawn >= _killsRequiredForElite)
        {
            _enemyKillsToCheckEliteSpawn = 0;
            if (_spawnEnemiesCount < maxEnemies)
            {
                SpawnEliteEnemy();
            }
        }


        // 남은 적 스폰
        if (_aliveEnemies < maxEnemies)
            StartSpawningForStage(_gameState.CurrentStage);



    }

    private int GetMaxEnemiesForStage(int stageIndex)
    {
        return (stageIndex >= 0 && stageIndex < _maxEnemiesByStage.Count) ? _maxEnemiesByStage[stageIndex] : 0;
    }

    private void StartSpawningForStage(int stageIndex)
    {
        if (!_isActive || _isSpawning) return;

        StopSpawnCoroutine();
        _spawnCoroutine = StartCoroutine(SpawnEnemies(stageIndex));
    }

    private IEnumerator SpawnEnemies(int stageIndex)
    {
        _isSpawning = true;
        int maxEnemies = GetMaxEnemiesForStage(stageIndex);

        //if (_spawn)

        while (_aliveEnemies < maxEnemies && _isActive && _spawnPoints.Count > 0)
        {
            if (_spawnEnemiesCount >= maxEnemies)
            {
                break;
            }
            if (GameStateManager.Instance.CurrentStage == GameStateManager.Instance.MaxStageNumberPerStage.Count - 1)
            {
                Debug.Log("보스방이다ㅏㅏㅏㅏ");
                yield return new WaitForSeconds(_spawnDelay);

                int spawnIdx = Random.Range(0, _spawnPoints.Count);
                Transform spawnPoint = _spawnPoints[spawnIdx];

                //여기서 대각선공격을 하게 되는데 이걸 조치하겟음 정욱
                GameObject enemy = EnemyPool.Instance.GetBossEnemy();
                enemy.transform.position = spawnPoint.position;
                enemy.transform.rotation = Quaternion.identity;
            }
            else
            {
                yield return new WaitForSeconds(_spawnDelay);

                int spawnIdx = Random.Range(0, _spawnPoints.Count);
                Transform spawnPoint = _spawnPoints[spawnIdx];

                //여기서 대각선공격을 하게 되는데 이걸 조치하겟음 정욱
                GameObject enemy = EnemyPool.Instance.GetEnemy();
                enemy.transform.position = spawnPoint.position;
                enemy.transform.rotation = Quaternion.identity;
            }

            _aliveEnemies++;

            _spawnEnemiesCount++;
        }
        _isSpawning = false;
            _spawnCoroutine = null;
    }

    private void SpawnEliteEnemy()
    {
        if (_spawnPoints.Count == 0) return;

        int spawnIdx = Random.Range(0, _spawnPoints.Count);
        Transform spawnPoint = _spawnPoints[spawnIdx];

        GameObject eliteEnemy = EnemyPool.Instance.GetEliteEnemy();
        eliteEnemy.transform.position = spawnPoint.position;
        eliteEnemy.transform.rotation = Quaternion.identity;

        _aliveEnemies++;
    }

    private void StopSpawnCoroutine()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
        _isSpawning = false;
    }

    //public void ChangeActive()
    //{

    //}

    public void SetActive(bool isActive)
    {
        _isActive = isActive;
        if (!_isActive) StopSpawnCoroutine();
        else StartSpawningForStage(_gameState.CurrentStage);
    }
}