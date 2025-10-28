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
    [SerializeField] private int _poolSize = 30; // �ӽ�ũ��

    private Queue<GameObject> _enemyPool = new Queue<GameObject>();
    private Queue<GameObject> _eliteEnemyPool = new Queue<GameObject>();

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

    // �Ϲ� �� ��������
    public GameObject GetEnemy()
    {
        GameObject obj = (_enemyPool.Count > 0) ? _enemyPool.Dequeue() : Instantiate(_enemyPrefab);
        obj.SetActive(true);
        return obj;
    }

    // ����Ʈ �� ��������
    public GameObject GetEliteEnemy()
    {
        GameObject obj = (_eliteEnemyPool.Count > 0) ? _eliteEnemyPool.Dequeue() : Instantiate(_eliteEnemyPrefab);
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
}