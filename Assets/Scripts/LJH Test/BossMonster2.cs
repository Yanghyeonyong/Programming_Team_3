using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster2 : MonoBehaviour
{
   


    [Header("Boss Stats")]
    [SerializeField][Range(0, 7000)] private int _maxHealth = 3000;
    [SerializeField][Range(0, 100)] private int _bossDefence = 50;
    [SerializeField][Range(0, 20)] private float _moveSpeed = 2f;
    [SerializeField] private Transform _player; // (����) Inspector�� �巡���ϼ���.
    private int currentHealth;
    private bool isDead = false;

    [Header("Attack Settings")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectilePoint;
    [SerializeField] private float _projectileSpeed = 10f;
    [SerializeField] private float _attackCooldown = 2f;

    [Header("FX & Sound")]
    // ... (�����ص� ��)

    [SerializeField] private Animator _bossAnimator;

    private void Awake()
    {
        // Animator �ڵ� �Ҵ� (Inspector�� �̹� ������ ����� ����)
        if (_bossAnimator == null)
        {
            _bossAnimator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        currentHealth = _maxHealth;
        Debug.Log("[Boss] Start: currentHealth = " + currentHealth);

        // Player �ڵ� Ž��(Inspector�� �ȳ־��� ���)
        if (_player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) _player = p.transform;
            if (_player != null) Debug.Log("[Boss] Player found and assigned.");
            else Debug.LogWarning("[Boss] Player Transform NOT assigned. Assign in Inspector or tag Player.");
        }

        if (_bossAnimator == null)
            Debug.LogWarning("[Boss] Animator is NULL. Attach Animator or assign in Inspector.");
    }

    private void Update()
    {
        // �Է� �����: �ֿܼ� �α� �������� Ȯ���ϼ���
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("[Boss] Space pressed detected.");
            if (_bossAnimator != null)
            {
                _bossAnimator.SetBool("isBossAttack", true);
                Debug.Log("[Boss] isBossAttack set to TRUE.");
            }
            else
            {
                Debug.LogWarning("[Boss] Animator is NULL - cannot SetBool.");
            }

            // ���� �߻� ȣ�� (���ϸ� �ּ� ����)
            SpawnProjectile();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("[Boss] F pressed detected.");
            if (_bossAnimator != null)
            {
                _bossAnimator.SetBool("isBossAttack", false);
                Debug.Log("[Boss] isBossAttack set to FALSE.");
            }
        }
    }

    public void SpawnProjectile()
    {
        if (_projectilePrefab == null)
        {
            Debug.LogWarning("[Boss] projectilePrefab is NULL - assign in Inspector.");
            return;
        }
        if (_projectilePoint == null)
        {
            Debug.LogWarning("[Boss] projectilePoint is NULL - assign in Inspector.");
            return;
        }

        GameObject projectile = Instantiate(_projectilePrefab, _projectilePoint.position, _projectilePoint.rotation);
        if (projectile == null)
        {
            Debug.LogWarning("[Boss] Instantiate failed (null).");
            return;
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = _projectilePoint.forward * _projectileSpeed;
            Debug.Log("[Boss] Projectile fired with velocity: " + rb.velocity);
        }
        else
        {
            Debug.LogWarning("[Boss] Projectile prefab has no Rigidbody! Add Rigidbody to prefab.");
        }
    }
}


