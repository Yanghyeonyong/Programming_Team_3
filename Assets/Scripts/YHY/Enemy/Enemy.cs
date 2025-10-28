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
    [SerializeField] private float _spellAttack;
    private bool _onAttack = false;

    //���� ���� ����
    [SerializeField] private float _attackRange;
    [SerializeField] private GameObject _spellObject;

    //���� ���� Ȱ��ȭ
    private bool _enableAttack = true;
    private bool _enableSpell = true;
    [SerializeField] private float _attackDelay;
    [SerializeField] private float _spellDelay;

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

    //���� ������ ��� üũ
    private bool _hitByWall = false;
    //���� ����(z�� ����) �Ѿ �� üũ
    private bool _hitByWallUp = false;

    //����ĳ��Ʈ �浹 ����
    private int layerMask;

    private AudioSource _audioSource;
    //0 : �ǰ�
    //1 : ���
    //2 : �ȱ�
    //3 : ����
    //4 : Ư�� ����(����)
    [SerializeField] List<AudioClip> _audioClips;

    private void Start()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
        //���̰� �÷��̾�� ���� �浹 üũ�ϵ��� ����
        layerMask = (1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("Wall"));

        _curHp = _maxHp;
        player = GameObject.Find("Player");
        if (_animator != null)
        {
            _animator.SetBool("_walk", false);
        }
        StartCoroutine(ChasePlayer());
    }

    //�÷��̾� ����
    IEnumerator ChasePlayer()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
        while (true)
        {

            Vector3 target = new Vector3(0, 0, 0);

            //���� �ε����� �ʾ��� ���
            if (_hitByWall == false && _onAttack == false)
            {
                //
                StartCoroutine(PlaySoundEffect(2));
                //
                _animator.SetBool("_walk", true);

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
            }
            else
            {
                _animator.SetBool("_walk", false);
            }

            Attack();

            if (_hitByWall == true && _hitByWallUp == true)
            {
                //���� ������ ���� �Ѿ���� ����
                target = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
                transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed);
            }
            else if (_hitByWall == true && _hitByWallUp == false)
            {
                //���� ������ ���� �Ѿ���� ����
                target = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
                transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed);
            }

            yield return null;
        }
    }

    //�� �浹�� �̵� ���� üũ
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            _hitByWall = true;
            if (collision.gameObject.transform.position.z < transform.position.z)
            {
                _hitByWallUp = true;
            }
            else
            {
                _hitByWallUp = false;
            }
        }
    }
    //�� �浹���� ��� ���
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            _hitByWall = false;
        }
    }

    //���� ���� ����
    private void Attack()
    {
        if (CheckPlayer())
        {
            if (_enableAttack == true && _onStun == false)
            {
                _onAttack = true;
                //�߰����� ������ ���� ���� ���
                if (_spellObject == null)
                {
                    //_onAttack = true;
                    _animator.SetTrigger("_attack");
                    StartCoroutine(PlayerAttack());
                    StartCoroutine(AttackDelay());
                }
                //������ ���� ���� ���
                else
                {
                    if (_enableSpell == true)
                    {
                        _animator.SetTrigger("_cast");
                        StartCoroutine(PlayerSpellAttack());
                        StartCoroutine(SpellDelay());
                    }
                    else
                    {
                        _animator.SetTrigger("_attack");
                        StartCoroutine(PlayerAttack());
                        StartCoroutine(AttackDelay());
                    }
                }
            }
        }
    }

    //�÷��̾ ���� ������ �ִ��� üũ
    private bool CheckPlayer()
    {
        Ray ray = new Ray(transform.position, transform.right * _attackRange);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _attackRange, layerMask))
        {
            //�÷��̾� ���̾� ���ڴ� 3
            if (hit.collider.gameObject.layer == 3)
            {
                return true;
            }
        }
        return false;
    }

    //�÷��̾�� ������ �ӵ� ����
    public void PlusMoveSpeed(float _plusMoveSpeed)
    {
        _moveSpeed += _plusMoveSpeed;
    }

    [SerializeField] private int _attackFrame = 1;
    [SerializeField] private int _spellFrame = 1;

    //���� ����
    IEnumerator PlayerSpellAttack()
    {
        //���� ������ �� �÷��̾ ������ ���ݹ����� ���� ��� ���� ����
        for (int i = 0; i < _spellFrame; i++)
        {
            yield return null;
        }

        //���� ����
        StartCoroutine(PlaySoundEffect(4));

        if (CheckPlayer())
        {
            _spellObject.transform.position = new Vector3(player.transform.position.x, _spellObject.transform.position.y, player.transform.position.z);
            _spellObject.SetActive(true);
            player.GetComponent<Player>().TakeDamage(_spellAttack);
        }
    }
    //�Ϲ� ����
    IEnumerator PlayerAttack()
    {

        //���� ������ �� �÷��̾ ������ ���ݹ����� ���� ��� ���� ����
        for (int i = 0; i < _attackFrame; i++)
        {
            yield return null;
        }

        //���� ����
        StartCoroutine(PlaySoundEffect(3));
        if (CheckPlayer())
        {
            player.GetComponent<Player>().TakeDamage(_attack);
        }
    }

    //���� ��Ÿ�� ����
    IEnumerator AttackDelay()
    {
        _enableAttack = false;
        yield return new WaitForSeconds(_attackDelay);
        _enableAttack = true;
        _onAttack = false;
    }

    //���� ��Ÿ�� ����
    IEnumerator SpellDelay()
    {

        _enableAttack = false;
        _enableSpell= false;
        yield return new WaitForSeconds(_attackDelay);

        _enableAttack = true;
        _onAttack= false;
        yield return new WaitForSeconds(_spellDelay-_attackDelay);
        _enableSpell= true;
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
        StartCoroutine(PlaySoundEffect(0));
        _curHp -= damage;
        StartCoroutine(StunDelay());
        if (_curHp <= 0)
        {
            _curHp = 0;
            StartCoroutine(Die());

        }
        else
        {
            _animator.SetTrigger("_hurt");
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
        StartCoroutine(PlaySoundEffect(1));
        StopCoroutine(ChasePlayer());
        _animator.Play("Death");


        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
        gameObject.SetActive(false);
    }


    IEnumerator PlaySoundEffect(int _audioNum)
    {
        if (_audioClips[_audioNum] != null)
        {

            Debug.Log("���� " + _audioNum);
            if (_audioNum == 2)
            {
                yield return !_audioSource.isPlaying;
                _audioSource.clip = _audioClips[_audioNum];
                _audioSource.Play();
            }
            else
            {
                //if (_audioSource.clip = _audioClips[3])
                //{
                //    _audioSource.Stop();
                //}
                _audioSource.PlayOneShot(_audioClips[_audioNum]);
            }
        }
    }
}