using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //������ ��ũ��Ʈ
    public int _spawnPos;
    // Start is called before the first frame update

    //���� Hp
    [SerializeField] private float _maxHp;
    [SerializeField] private float _curHp;

    //���� ���ݷ�
    [SerializeField] private float _attack;

    //���� ���� ����
    [SerializeField] private float _attackRange;

    //���� ��Ÿ�� üũ
    private bool _enableAttack=true;
    [SerializeField] private float _attackDelay;

    //�ǰݽ� ���� �ð�
    [SerializeField] private float _stunDelay;
    private bool _onStun=false;
    
    //���� �̵� �ӵ�
    [SerializeField] private float _moveSpeed;
    //�÷��̾������ �Ÿ� ����
    [SerializeField] private float _moveDistance;

    //�÷��̾�
    GameObject player;

    //�ִϸ��̼� ����� �ִϸ�����
    Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    //��ü Ȱ��ȭ ��
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

    //�÷��̾ �����ϴ� �Լ�
    IEnumerator ChasePlayer()
    {
        while (true)
        {
            Vector3 target = new Vector3(0, 0, 0);

            //�÷��̾��� ��ġ�� ���� ���� ����
            if (player.transform.position.x > transform.position.x)
            {
                //�÷��̾ ���߿� ���� ��� ���ư��� �� ����
                target = new Vector3(player.transform.position.x - _moveDistance, transform.position.y, player.transform.position.z);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else 
            {
                target = new Vector3(player.transform.position.x + _moveDistance, transform.position.y, player.transform.position.z);
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }

            //�÷��̾ ���� ����̵�
            transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed);
            yield return null;

            AttackPlayer();
        }
    }

    //�÷��̾� ���� �Լ�
    private void AttackPlayer()
    {
        Ray ray = new Ray(transform.position, transform.right*_attackRange);
        RaycastHit hit;
 
        if (Physics.Raycast(ray, out hit, _attackRange))
        {
            if(hit.collider.gameObject.CompareTag("Player")&&_enableAttack==true) 
            {
                Debug.Log("����");
                _animator.Play("Attack");
                StartCoroutine(AttackDelay());
            }
        }
    }

    //��ü ���� ������ �÷��̾� �߰� �� �ӵ� ���� �Լ�
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

    //���� ������ ��� �Լ�
    IEnumerator AttackDelay()
    {
        _enableAttack = false;
        yield return new WaitForSeconds(_attackDelay);
        _enableAttack = true;
        Debug.Log("üũ");


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnTakeDamage(3);
        }
    }

    //�ǰݽ� Hp ���� �Լ�
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

    //�ǰݽ� �������� ���� ���� �Ұ� �ð� ��� �Լ�
    IEnumerator StunDelay()
    {
        _onStun = true;
        yield return new WaitForSeconds(_stunDelay);
        _onStun = false;
    }

    //��ü ����� ȣ�� �Լ�
    IEnumerator Die()
    {
        StopCoroutine(ChasePlayer());
        _animator.Play("Death");

        //�ִϸ��̼� ���̸�ŭ ����
        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
        gameObject.SetActive(false);
    }
}
