using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//dd
public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    [Header("Prefab & Pool Settings")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _eliteEnemyPrefab;
    [SerializeField] private GameObject _bossEnemyPrefab;
    [SerializeField] private int _poolSize = 30; // �ӽ�ũ��

    private Queue<GameObject> _enemyPool = new Queue<GameObject>();
    private Queue<GameObject> _eliteEnemyPool = new Queue<GameObject>();
    private Queue<GameObject> _bossEnemyPool = new Queue<GameObject>();
    private void Awake()
    {
        // �̱��� �ʱ�ȭ
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // �Ϲ� �� Ǯ ����
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject enemy = Instantiate(_enemyPrefab);
            enemy.SetActive(false);
            _enemyPool.Enqueue(enemy);
        }

        // ����Ʈ �� Ǯ ���� (Ǯ ũ�� 1/3)
        for (int i = 0; i < _poolSize / 3; i++)
        {
            GameObject eliteEnemy = Instantiate(_eliteEnemyPrefab);
            eliteEnemy.SetActive(false);
            _eliteEnemyPool.Enqueue(eliteEnemy);
        }
    }

    //�밢�� ���ݹ��� ���� isntatiate ��ġ�� ������ �س��ߵ�

    

    // �Ϲ� �� ��������
    public GameObject GetEnemy()
    {
        GameObject obj = (_enemyPool.Count > 0) ? _enemyPool.Dequeue() : Instantiate(_enemyPrefab, new Vector3(0,-10f,0),_enemyPrefab.transform.rotation);
        obj.SetActive(true);
        return obj;
    }

    // ����Ʈ �� ��������
    public GameObject GetEliteEnemy()
    {
        GameObject obj = (_eliteEnemyPool.Count > 0) ? _eliteEnemyPool.Dequeue() : Instantiate(_eliteEnemyPrefab, new Vector3(0, -10f, 0), _eliteEnemyPrefab.transform.rotation);
        obj.SetActive(true);
        return obj;
    }

    // �Ϲ� �� ��ȯ
    public void ReturnEnemy(GameObject obj)
    {
        obj.SetActive(false);
        if (!_enemyPool.Contains(obj))
            _enemyPool.Enqueue(obj);
    }

    // ����Ʈ �� ��ȯ
    public void ReturnEliteEnemy(GameObject obj)
    {
        obj.SetActive(false);
        if (!_eliteEnemyPool.Contains(obj))
            _eliteEnemyPool.Enqueue(obj);
    }


    public GameObject GetBossEnemy()
    {
        GameObject obj = (_bossEnemyPool.Count > 0) ? _bossEnemyPool.Dequeue() : Instantiate(_bossEnemyPrefab, new Vector3(0, -10f, 0), _bossEnemyPrefab.transform.rotation);
        obj.SetActive(true);
        GameStateManager.Instance._isBossAlive = true;
        return obj;
    }

    public void ReturnBossEnemy(GameObject obj)
    {
        obj.SetActive(false);
        if (!_bossEnemyPool.Contains(obj))
            _bossEnemyPool.Enqueue(obj);
    }
}
//