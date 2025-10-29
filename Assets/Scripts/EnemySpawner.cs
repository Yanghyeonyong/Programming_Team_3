using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //싱글톤 인스턴스
    public static EnemySpawner Instance;

    [Header("Prefabs")] // 잡몹, 정예몹 프리팹 참조
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _eliteEnemyPrefab;


    [Header("Spawn Settings")] // 정예 몬스터 스폰조건, 스폰주기, 스폰 여부
    [SerializeField] private int _killsRequiredForElite = 5;
    [SerializeField] private float _spawnDelay = 2f;
    [SerializeField] private bool _isActive = false;


    //중복 스폰 방지
    private bool _isSpawning = false;

    //정예 몬스터가 이미 소환되었는지 여부
    private bool _isEliteSpawned = false;

    //현재 스테이지에서 살아있는 적 수
    private int _aliveEnemies;

    //현재 스테이지에서 잡은 총 적 수
    private int _totalKillsInStage;

    //각 스테이지별 최대 적 수 (GameStateManeger에서 가져옴)
    private List<int> _maxEnemiesByStage;

    //스폰 위치 리스트
    private List<Transform> _spawnPoints = new List<Transform>();

    //GameStateManager 캐싱
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

        //하위 오브젝트를 스폰 포인트로 등록
        foreach (Transform child in transform)
        {
            _spawnPoints.Add(child);
        }

    }

    // 게임 상태 매니저의 이벤트 구독
    private void Start()
    {
        _gameState = GameStateManager.Instance; // 한 번만 참조.

        _gameState.OnStageChanged.AddListener(OnStageChanged);
         _gameState.OnEnemyDied.AddListener(OnEnemyDied);

         _maxEnemiesByStage = _gameState.GetMaxEnemiesList();
        _isActive = true;
        StartCoroutine(SpawnEnemies(0));
    }

    // 이벤트 구독해제
    private void OnDestroy()
    {
        if (_gameState != null)
        {
           _gameState.OnStageChanged.RemoveListener(OnStageChanged);
            _gameState.OnEnemyDied.RemoveListener(OnEnemyDied);
        }
    }

    //스테이지 변경 시 스폰 시작
    private void OnStageChanged(int stageIndex)
    {
        _aliveEnemies = 0;
        _totalKillsInStage = 0;
        _isEliteSpawned = false;

        StopCoroutine(SpawnEnemies(stageIndex));
        StartCoroutine(SpawnEnemies(stageIndex));
    }

    //적 사망 시 호출되어 현재 적 수 감소
    private void OnEnemyDied()
    {
       
        _aliveEnemies -= 1;
        _totalKillsInStage += 1;

        int maxEnemies = _maxEnemiesByStage[(int)_gameState.CurrentStage];

        //일정 수 처치 시 정예 몬스터 등장
        if(_isEliteSpawned == false && _totalKillsInStage >= _killsRequiredForElite)
        {
            _isEliteSpawned = true;
            SpawnEliteEnemy();
        }
        
        //일반 몬스터 스폰 유지
        if (_aliveEnemies < maxEnemies)
        {
            StartCoroutine(SpawnEnemies(_gameState.CurrentStage));
        }

    }

    //적 스폰 코루틴
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

    //정예 몬스터 스폰 메서드
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
