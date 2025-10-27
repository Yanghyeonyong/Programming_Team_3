using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //�̱��� �ν��Ͻ�
    public static EnemySpawner Instance;

    [Header("Prefabs")] // ���, ������ ������ ����
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _eliteEnemyPrefab;


    [Header("Spawn Settings")] // ���� ���� ��������, �����ֱ�, ���� ����
    [SerializeField] private int _killsRequiredForElite = 5;
    [SerializeField] private float _spawnDelay = 2f;
    [SerializeField] private bool _isActive = false;


    //�ߺ� ���� ����
    private bool _isSpawning = false;

    //���� ���Ͱ� �̹� ��ȯ�Ǿ����� ����
    private bool _isEliteSpawned = false;

    //���� ������������ ����ִ� �� ��
    private int _aliveEnemies;

    //���� ������������ ���� �� �� ��
    private int _totalKillsInStage;

    //�� ���������� �ִ� �� �� (GameStateManeger���� ������)
    private List<int> _maxEnemiesByStage;

    //���� ��ġ ����Ʈ
    private List<Transform> _spawnPoints = new List<Transform>();

    //GameStateManager ĳ��
    private GameStateManager _gameState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //���� ������Ʈ�� ���� ����Ʈ�� ���
        foreach (Transform child in transform)
        {
            _spawnPoints.Add(child);
        }

    }

    // ���� ���� �Ŵ����� �̺�Ʈ ����
    private void Start()
    {
        _gameState = GameStateManager.Instance; // �� ���� ����.

        _gameState.OnStageChanged.AddListener(OnStageChanged);
         _gameState.OnEnemyDied.AddListener(OnEnemyDied);

         _maxEnemiesByStage = _gameState.GetMaxEnemiesList();
        _isActive = true;
        StartCoroutine(SpawnEnemies(0));
    }

    // �̺�Ʈ ��������
    private void OnDestroy()
    {
        if (_gameState != null)
        {
           _gameState.OnStageChanged.RemoveListener(OnStageChanged);
            _gameState.OnEnemyDied.RemoveListener(OnEnemyDied);
        }
    }

    //�������� ���� �� ���� ����
    private void OnStageChanged(int stageIndex)
    {
        _aliveEnemies = 0;
        _totalKillsInStage = 0;
        _isEliteSpawned = false;

        StopCoroutine(SpawnEnemies(stageIndex));
        StartCoroutine(SpawnEnemies(stageIndex));
    }

    //�� ��� �� ȣ��Ǿ� ���� �� �� ����
    private void OnEnemyDied()
    {
       
        _aliveEnemies -= 1;
        _totalKillsInStage += 1;

        int maxEnemies = _maxEnemiesByStage[(int)_gameState.CurrentStage];

        //���� �� óġ �� ���� ���� ����
        if(_isEliteSpawned == false && _totalKillsInStage >= _killsRequiredForElite)
        {
            _isEliteSpawned = true;
            SpawnEliteEnemy();
        }
        
        //�Ϲ� ���� ���� ����
        if (_aliveEnemies < maxEnemies)
        {
            StartCoroutine(SpawnEnemies(_gameState.CurrentStage));
        }

    }

    //�� ���� �ڷ�ƾ
    private IEnumerator SpawnEnemies(int stageIndex)
    {
        if (_isActive == false || _isSpawning)
        {
            yield break;
        }

        _isSpawning = true;

        int maxEnemies = _maxEnemiesByStage[(int)stageIndex];

        while (_aliveEnemies < maxEnemies)
        {
            yield return new WaitForSeconds(_spawnDelay);

            Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
            Instantiate(_enemyPrefab, spawnPoint.position, Quaternion.identity);
            _aliveEnemies++;
        }

        _isSpawning = false;
    }

    //���� ���� ���� �޼���
    private void SpawnEliteEnemy()
    {
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
        Instantiate(_eliteEnemyPrefab, spawnPoint.position, Quaternion.identity);
    }


    public void SetActive(bool isActive)
    {
        _isActive = isActive;
    }


}
