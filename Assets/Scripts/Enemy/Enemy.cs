using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //슬라임 스크립트
    public int _spawnPos;
    // Start is called before the first frame update

    //몬스터 Hp
    [SerializeField] private float _maxHp;
    [SerializeField] private float _curHp;

    //몬스터 공격력
    [SerializeField] private float _attack;

    //몬스터 공격 범위
    [SerializeField] private float _attackRange;

    //공격 쿨타임 체크
    private bool _enableAttack=true;
    [SerializeField] private float _attackDelay;

    //피격시 경직 시간
    [SerializeField] private float _stunDelay;
    private bool _onStun=false;
    
    //몬스터 이동 속도
    [SerializeField] private float _moveSpeed;
    //플레이어까지의 거리 범위
    [SerializeField] private float _moveDistance;

    //플레이어
    GameObject player;

    //애니메이션 실행용 애니메이터
    Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    //객체 활성화 시
    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        _curHp = _maxHp;
        player = GameObject.Find("Player");
        StartCoroutine(ChasePlayer());
    }

    //플레이어를 추적하는 함수
    IEnumerator ChasePlayer()
    {
        while (true)
        {
            Vector3 target = new Vector3(0, 0, 0);

            //플레이어의 위치에 따른 범위 지정
            if (player.transform.position.x > transform.position.x)
            {
                //플레이어가 공중에 있을 경우 날아가는 것 방지
                target = new Vector3(player.transform.position.x - _moveDistance, transform.position.y, player.transform.position.z);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else 
            {
                target = new Vector3(player.transform.position.x + _moveDistance, transform.position.y, player.transform.position.z);
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }

            //플레이어를 향해 등속이동
            transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed);
            yield return null;

            AttackPlayer();
        }
    }

    //플레이어 공격 함수
    private void AttackPlayer()
    {
        Ray ray = new Ray(transform.position, transform.right*_attackRange);
        RaycastHit hit;
 
        if (Physics.Raycast(ray, out hit, _attackRange))
        {
            if(hit.collider.gameObject.CompareTag("Player")&&_enableAttack==true) 
            {
                Debug.Log("공격");
                _animator.Play("Attack");
                StartCoroutine(AttackDelay());
            }
        }
    }

    //객체 감지 범위에 플레이어 발견 시 속도 증가 함수
    public void PlusMoveSpeed(float _plusMoveSpeed)
    {
        _moveSpeed += _plusMoveSpeed;
    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player")==false)
    //    {
    //        //_enableAttack = false;
    //        //StartCoroutine(AttackDelay());
    //        //AttackPlayer();
    //        if (player.transform.position.z > transform.position.z)
    //        {
    //            transform.Translate(-Vector3.forward * Time.deltaTime*_moveSpeed);
    //        }
    //        else
    //        {
    //            transform.Translate(Vector3.forward * Time.deltaTime * _moveSpeed);
    //        }
    //    }
    //}

    //공격 딜레이 계산 함수
    IEnumerator AttackDelay()
    {
        _enableAttack = false;
        yield return new WaitForSeconds(_attackDelay);
        _enableAttack = true;
        Debug.Log("체크");


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnTakeDamage(3);
        }
    }

    //피격시 Hp 감소 함수
    public void OnTakeDamage(float damage)
    {
        _curHp -= damage;
        StartCoroutine(StunDelay());
        if (_curHp <= 0)
        {
            _curHp = 0;
            StartCoroutine(Die());
        }
        else
        {
            _animator.Play("Hurt");
        }
    }

    //피격시 경직으로 인한 공격 불가 시간 계산 함수
    IEnumerator StunDelay()
    {
        _onStun = true;
        yield return new WaitForSeconds(_stunDelay);
        _onStun = false;
    }

    //객체 사망시 호출 함수
    IEnumerator Die()
    {
        StopCoroutine(ChasePlayer());
        _animator.Play("Death");

        //애니메이션 길이만큼 실행
        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
        gameObject.SetActive(false);
    }
}
