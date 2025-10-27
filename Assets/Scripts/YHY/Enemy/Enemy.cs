using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    //몬스터 Hp
    [SerializeField] private float _maxHp;
    [SerializeField] private float _curHp;

    //몬스터 공격력
    [SerializeField] private float _attack;

    //몬스터 공격 범위
    [SerializeField] private float _attackRange;

    //몬스터 공격 활성화
    private bool _enableAttack = true;
    [SerializeField] private float _attackDelay;

    //몬스터 피격 후 경직
    [SerializeField] private float _stunDelay;
    private bool _onStun = false;

    //몬스터 이동 속도
    [SerializeField] private float _moveSpeed;
    //몬스터와 플레이어 거리
    [SerializeField] private float _moveDistance;

    //플레이어
    GameObject player;

    //애니메이션을 위한 애니메이터
    Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

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

    //플레이어 추적
    IEnumerator ChasePlayer()
    {
        while (true)
        {
            Vector3 target = new Vector3(0, 0, 0);

            //플레이어의 앞 뒤일 경우 각각 적용
            if (player.transform.position.x > transform.position.x)
            {
                target = new Vector3(player.transform.position.x - _moveDistance, transform.position.y, player.transform.position.z);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                target = new Vector3(player.transform.position.x + _moveDistance, transform.position.y, player.transform.position.z);
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }

            //y축 이동 방지
            transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed);
            yield return null;

            AttackPlayer();
        }
    }

    //플레이어 공격
    private void AttackPlayer()
    {
        Ray ray = new Ray(transform.position, transform.right * _attackRange);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _attackRange))
        {
            if (hit.collider.gameObject.CompareTag("Player") && _enableAttack == true)
            {
                _animator.Play("Attack");
                StartCoroutine(AttackDelay());
            }
        }
    }

    //플레이어와 근접시 속도 증가
    public void PlusMoveSpeed(float _plusMoveSpeed)
    {
        _moveSpeed += _plusMoveSpeed;
    }


    //공격 딜레이 적용
    IEnumerator AttackDelay()
    {
        _enableAttack = false;
        yield return new WaitForSeconds(_attackDelay);
        _enableAttack = true;


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnTakeDamage(3);
        }
    }

    //데미지를 입음
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

    //피격시 공격 불가
    IEnumerator StunDelay()
    {
        _onStun = true;
        yield return new WaitForSeconds(_stunDelay);
        _onStun = false;
    }

    //몬스터 사망
    IEnumerator Die()
    {
        StopCoroutine(ChasePlayer());
        _animator.Play("Death");

       
        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
        gameObject.SetActive(false);
    }
}