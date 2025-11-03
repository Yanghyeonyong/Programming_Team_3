using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossMonster : MonoBehaviour
{

    [Header("Boss Stats")] //보스 몬스터의 상태
    [SerializeField][Range(0, 7000)] private int _maxHealth = 3000; //모든 설정값은 임의 플레이어 밸런스에 맞게 조정가능
    [SerializeField][Range(0, 100)] private int _bossDefence = 50; //방어력
    [SerializeField][Range(0, 20)] private float _moveSpeed = 2f; //이동속도 플레이어보다 느리게 조정가능
    [SerializeField] private Transform _player; //플레이어의 위치
    private int currentHealth; //현재 체력
    private bool isDead = false; // 살아있음

    [Header("Attack Settings")] //보스 공격패턴 구성
    [SerializeField] private GameObject _projectilePrefab; //평타 원거리발사체
    [SerializeField] private Transform _projectilePoint; //원거리발사체 지점, 위치
    [SerializeField] private float _projectileSpeed = 10; //원거리발사체 스피드
    [SerializeField] private float _attackCooldown = 2f; //보스평타 원거리발사체 쿨다운 조정가능

    [SerializeField] private GameObject _homingFireball; // 1단계 적용시 따라가는 유도탄 파이어볼
    [SerializeField] private GameObject _monsterSkeleton; //해골이나 소환물 벨런스에 맞게 소환
    [SerializeField][Range(0, 5)] private float _summonPosition; //소환 위치
    

    [Header("FX & Sound")] //이팩트 및 사운드 관리
    [SerializeField] private GameObject _bossSpawnEffect; //등장 이팩트 효과
    [SerializeField] private AudioSource _bgmSource; //테마 배경음
    [SerializeField] private AudioClip _bossAttack; // 원거리발사체 평타
    [SerializeField] private AudioClip _bossDamage; // 피격 받았을때 
    [SerializeField] private AudioClip _fireballSound; // 1번째 스킬 유도파이어볼
    [SerializeField] private AudioClip _summonSound; //소환스킬 사용시 (중간중간 벨런스에 사용)
    [SerializeField] private AudioClip _deathSound; //죽었을 경우
    [SerializeField] private AudioClip _Explosion; //2번째 스킬 지면 폭발



    private Queue<GameObject> _monsters = new Queue<GameObject>(); //소환물 담을거

    // [SerializeField] private PlayerController _player; 플레이어 어떻게 참조 해야 하는지

    [SerializeField] private Animator _bossAnimator; //애니매이션 관리



    




    private void Start()
    {
        currentHealth = _maxHealth; // 보스체력  



        _bossAnimator = GetComponent<Animator>();


    }

    private void Update()
    {
       //
       // if (Input.GetKeyDown(KeyCode.Space)) //이건 지금 반응이 없어서 해보는중
       // {
       //     //SpawnProjectile();
       //
       //     _bossAnimator.SetBool("isBossAttack", true); //_player.IsAlive); //우선 테스트 위해서 코드 주석
       //
       // }
       // if (Input.GetKeyDown(KeyCode.F))
       // {
       //
       //     _bossAnimator.SetBool("isBossAttack", false);
       //
       // }



        // if (_bossAnimator == null || _player == null) return; //플레이어 변수명 확인 

        // 플레이어가 살아있을 때만 공격 애니메이션 실행



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
    //         rb.velocity = _projectilePoint.forward * _projectileSpeed; // forward 방향으로 발사
    //     }
    //     
    //
    //     Debug.Log("파이어볼 테스트 로고");
    //








    /* public void OnBossSpawn()
    {
       // Instantiate( transform.position, Quaternion.identity); /




        Debug.Log("보스 등장! 위자드: '이곳은 나의 영역이다'");
    }

    private void Spawn() // 해골 몬스터 소환
    {
        GameObject monster = Instantiate(_monsterSkeleton);


    } */


    // _projectilePoint.LookAt(_player); // 플레이어 방향 바라보게
    // Rigidbody rb = projectile.GetComponent<Rigidbody>();
    // rb.velocity = _projectilePoint.forward * _projectileSpeed;
    //


 }





