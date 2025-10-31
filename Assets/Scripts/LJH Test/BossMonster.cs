using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossMonster : MonoBehaviour
{

    [Header("Boss Stats")] //���� ������ ����
    [SerializeField][Range(0, 7000)] private int _maxHealth = 3000; //��� �������� ���� �÷��̾� �뷱���� �°� ��������
    [SerializeField][Range(0, 100)] private int _bossDefence = 50; //����
    [SerializeField][Range(0, 20)] private float _moveSpeed = 2f; //�̵��ӵ� �÷��̾�� ������ ��������
    [SerializeField] private Transform _player; //�÷��̾��� ��ġ
    private int currentHealth; //���� ü��
    private bool isDead = false; // �������

    [Header("Attack Settings")] //���� �������� ����
    [SerializeField] private GameObject _projectilePrefab; //��Ÿ ���Ÿ��߻�ü
    [SerializeField] private Transform _projectilePoint; //���Ÿ��߻�ü ����, ��ġ
    [SerializeField] private float _projectileSpeed = 10; //���Ÿ��߻�ü ���ǵ�
    [SerializeField] private float _attackCooldown = 2f; //������Ÿ ���Ÿ��߻�ü ��ٿ� ��������

    [SerializeField] private GameObject _homingFireball; // 1�ܰ� ����� ���󰡴� ����ź ���̾
    [SerializeField] private GameObject _monsterSkeleton; //�ذ��̳� ��ȯ�� �������� �°� ��ȯ
    [SerializeField][Range(0, 5)] private float _summonPosition; //��ȯ ��ġ
    

    [Header("FX & Sound")] //����Ʈ �� ���� ����
    [SerializeField] private GameObject _bossSpawnEffect; //���� ����Ʈ ȿ��
    [SerializeField] private AudioSource _bgmSource; //�׸� �����
    [SerializeField] private AudioClip _bossAttack; // ���Ÿ��߻�ü ��Ÿ
    [SerializeField] private AudioClip _bossDamage; // �ǰ� �޾����� 
    [SerializeField] private AudioClip _fireballSound; // 1��° ��ų �������̾
    [SerializeField] private AudioClip _summonSound; //��ȯ��ų ���� (�߰��߰� �������� ���)
    [SerializeField] private AudioClip _deathSound; //�׾��� ���
    [SerializeField] private AudioClip _Explosion; //2��° ��ų ���� ����



    private Queue<GameObject> _monsters = new Queue<GameObject>(); //��ȯ�� ������

    // [SerializeField] private PlayerController _player; �÷��̾� ��� ���� �ؾ� �ϴ���

    [SerializeField] private Animator _bossAnimator; //�ִϸ��̼� ����



    




    private void Start()
    {
        currentHealth = _maxHealth; // ����ü��  



        _bossAnimator = GetComponent<Animator>();


    }

    private void Update()
    {
       //
       // if (Input.GetKeyDown(KeyCode.Space)) //�̰� ���� ������ ��� �غ�����
       // {
       //     //SpawnProjectile();
       //
       //     _bossAnimator.SetBool("isBossAttack", true); //_player.IsAlive); //�켱 �׽�Ʈ ���ؼ� �ڵ� �ּ�
       //
       // }
       // if (Input.GetKeyDown(KeyCode.F))
       // {
       //
       //     _bossAnimator.SetBool("isBossAttack", false);
       //
       // }



        // if (_bossAnimator == null || _player == null) return; //�÷��̾� ������ Ȯ�� 

        // �÷��̾ ������� ���� ���� �ִϸ��̼� ����



        // if (_bossAnimator != null)

        //
        // {
        //     _bossAnimator.SetBool("isBossAttack", true); 
        // 
        // }
        //
        // if (_bossAnimator != null)
        // { 
        // 
        //    _bossAnimator.SetBool("isBossAttack", false);
        // 
        // }
        //

    }


    // public void SpawnProjectile()
    // {
    //    // if (_projectilePrefab == null || _projectilePoint == null) return;
    //
    //     GameObject projectile = Instantiate(_projectilePrefab, _projectilePoint.position, _projectilePoint.rotation);
    //
    //
    //     Rigidbody rb = projectile.GetComponent<Rigidbody>();
    //     if (rb != null)
    //     {
    //         rb.velocity = _projectilePoint.forward * _projectileSpeed; // forward �������� �߻�
    //     }
    //     
    //
    //     Debug.Log("���̾ �׽�Ʈ �ΰ�");
    //








    /* public void OnBossSpawn()
    {
       // Instantiate( transform.position, Quaternion.identity); /




        Debug.Log("���� ����! ���ڵ�: '�̰��� ���� �����̴�'");
    }

    private void Spawn() // �ذ� ���� ��ȯ
    {
        GameObject monster = Instantiate(_monsterSkeleton);


    } */


    // _projectilePoint.LookAt(_player); // �÷��̾� ���� �ٶ󺸰�
    // Rigidbody rb = projectile.GetComponent<Rigidbody>();
    // rb.velocity = _projectilePoint.forward * _projectileSpeed;
    //


 }





