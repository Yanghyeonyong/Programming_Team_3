using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    //���� Hp
    [SerializeField] private float _maxHp;
    [SerializeField] private float _curHp;

    //���� ���ݷ�
    [SerializeField] private float _attack;

    //���� ���� ����
    [SerializeField] private float _attackRange;

    //���� ���� Ȱ��ȭ
    private bool _enableAttack = true;
    [SerializeField] private float _attackDelay;

    //���� �ǰ� �� ����
    [SerializeField] private float _stunDelay;
    private bool _onStun = false;

    //���� �̵� �ӵ�
    [SerializeField] private float _moveSpeed;
    //���Ϳ� �÷��̾� �Ÿ�
    [SerializeField] private float _moveDistance;

    //�÷��̾�
    GameObject player;

    //�ִϸ��̼��� ���� �ִϸ�����
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

    //�÷��̾� ����
    IEnumerator ChasePlayer()
    {
        while (true)
        {
            Vector3 target = new Vector3(0, 0, 0);

            //�÷��̾��� �� ���� ��� ���� ����
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

            //y�� �̵� ����
            transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed);
            yield return null;

            AttackPlayer();
        }
    }

    //�÷��̾� ����
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

    //�÷��̾�� ������ �ӵ� ����
    public void PlusMoveSpeed(float _plusMoveSpeed)
    {
        _moveSpeed += _plusMoveSpeed;
    }


    //���� ������ ����
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

    //�������� ����
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

    //�ǰݽ� ���� �Ұ�
    IEnumerator StunDelay()
    {
        _onStun = true;
        yield return new WaitForSeconds(_stunDelay);
        _onStun = false;
    }

    //���� ���
    IEnumerator Die()
    {
        StopCoroutine(ChasePlayer());
        _animator.Play("Death");

       
        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
        gameObject.SetActive(false);
    }
}