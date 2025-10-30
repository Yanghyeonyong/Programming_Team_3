using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    //���� Hp
    [SerializeField] private float _maxHp;
    [SerializeField] protected float _curHp;

    //���� ���ݷ�
    [SerializeField] private float _attack;
    [SerializeField] private float _spellAttack;
    protected bool _onAttack = false;

    //���� ���� ����
    [SerializeField] private float _attackRange;
    [SerializeField] protected GameObject _spellObject;

    //���� ���� Ȱ��ȭ
    protected bool _enableAttack = true;
    protected bool _enableSpell = true;
    [SerializeField] private float _attackDelay;
    [SerializeField] private float _spellDelay;

    //���� �ǰ� �� ����
    [SerializeField] private float _stunDelay;
    private bool _onStun = false;

    //���� �̵� �ӵ�
    [SerializeField] private float _moveSpeed;
    //���Ϳ� �÷��̾� �Ÿ�
    [SerializeField] private float _moveDistance;

    //������ �浹 ƨ���������� �� ����, ���� Ÿ�̹�, ���ܽ� ������ ����
    private Rigidbody _rigidbody;
    [SerializeField] private float _isBlockedCheckDistance = 3f;
    [SerializeField] private float _attackRayTiming = 1f;
    BoxCollider _enemyCollider;
    float _colliderSize;
    float _moveDistancePreserve;
    [SerializeField] private float _attackDashCloseDistance = 0.65f;

    //�÷��̾�
    GameObject player;

    //�ִϸ��̼��� ���� �ִϸ�����
    protected Animator _animator;

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


    //���� ���� üũ, �Ϲ��� �ƹ��͵� üũ x

    [SerializeField] private bool _isElite = false;
    [SerializeField] private bool _boss = false;
    [SerializeField] private bool _summon = false;
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
        StartCoroutine(FindPlayer());
        _rigidbody = GetComponent<Rigidbody>();
        _enemyCollider = GetComponent<BoxCollider>();
        gameObject.GetComponent<BoxCollider>().enabled = true;
        _colliderSize = _enemyCollider.size.x;
        _moveDistancePreserve = _moveDistance;
        //Init();
    }

    IEnumerator FindPlayer()
    {
        while (player == null)
        {
            if (FindObjectOfType<Player>() != null)
            {
                player = FindObjectOfType<Player>().gameObject;
                break;
            }
            yield return null;
        }
        //Debug.Log($"�÷��̾� ã�� {player.name}");
        Init();
        //Debug.Log("�� �ʱ�ȭ �Ϸ�");
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
        //StartCoroutine(FindPlayer());

        //player = FindObjectOfType<Player>().gameObject;
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
            //���� ���� �߿��� �����̴°� ������ ���� _onAttack = false�� ���� �����̵���
            if (_hitByWall == false && !CheckPlayer() && _onAttack == false)
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

                //float _distance = Vector3.Distance(transform.position, target);
                Vector3 _direction = (target - transform.position).normalized;

                if (Physics.Raycast(transform.position + Vector3.up * 0.2f, _direction, out RaycastHit hit, _isBlockedCheckDistance, layerMask + (1 << LayerMask.NameToLayer("Enemy"))))

                {
                    if (!hit.collider.gameObject.CompareTag("Wall"))
                    //Debug.Log("�� �տ��� ����");
                    {
                        target = transform.position;
                    }
                }
                //y�� �̵� ����
                if (_isPlayerAttack == false)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed);

                }
            }
            else
            {
                _animator.SetBool("_walk", false);
            }

            //Debug.Log("right before attack");
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

        if (_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
        //StopCoroutine(ChasePlayer());

    }

    IEnumerator StopMoving()
    {
        while (true)
        {
            if (_rigidbody != null)
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
            yield return null;


            //yield return 
        }

    }

    void OnCollisionStay(Collision collision)
    {

        if (_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }



    }
    //�� �浹���� ��� ���
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            _hitByWall = false;
        }


        //_rigidbody = GetComponent<Rigidbody>();



        //StartCoroutine(ChasePlayer());

    }



    //���� ���� ����
    private void Attack()
    {
        //Debug.Log("attack function called");
        if (CheckPlayer())
        {
            //Debug.Log("player in attack range");
            if (_enableAttack == true && _onStun == false)
            {
                _onAttack = true;
                //�߰����� ������ ���� ���� ���
                if (_spellObject == null)
                {
                    //_isAttacking = true;
                    _animator.SetTrigger("_attack");

                    StartCoroutine("PlayerAttack");
                    StartCoroutine("AttackDelay");
                }
                //������ ���� ���� ���
                else
                {
                    if (_enableSpell == true)
                    {
                        _animator.SetTrigger("_cast");
                        StartCoroutine("PlayerSpellAttack");
                        StartCoroutine("SpellDelay");
                    }
                    else
                    {
                        _animator.SetTrigger("_attack");
                        StartCoroutine("PlayerAttack");
                        StartCoroutine("AttackDelay");
                    }
                }
            }
        }
    }



    //�÷��̾ ���� ������ �ִ��� üũ
    private bool CheckPlayer()
    {

        Ray ray = new Ray(transform.position + Vector3.up * 0.2f, transform.right * _attackRange);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _attackRange, layerMask))
        {
            //�÷��̾� ���̾� ���ڴ� 3
            if (hit.collider.gameObject.layer == 3)
            {
                return true;
            }
        }
        else
        {
            //Debug.Log("�ƹ��͵� �ȸ���");
        }
        return false;
    }

    //�÷��̾�� ������ �ӵ� ����
    public void PlusMoveSpeed(float _plusMoveSpeed)
    {
        _moveSpeed += _plusMoveSpeed;
    }


    protected bool _isPlayerSpellAttack = false;
    [SerializeField] private int _attackFrame = 1;
    [SerializeField] protected int _spellFrame = 1;

    //���� ����
    IEnumerator PlayerSpellAttack()
    {
        _isPlayerSpellAttack = true;

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
        _onAttack = false;
        _isPlayerSpellAttack = false;
    }

    [SerializeField] private float _attackDashTimeAndDamageTimer = 0.5f;
    protected bool _isPlayerAttack = false;

    //�����ϸ鼭 �÷��̾� ��������� �ְ�

    IEnumerator AttackGettingCloseToPlayer()
    {
        Vector3 _tempPlayerLocation = player.transform.position;
        yield return new WaitForSeconds(0.9f);
        float _currentTime = 0f;
        while (_currentTime < _attackDashTimeAndDamageTimer && _attackDashCloseDistance <= Vector3.Distance(player.transform.position, transform.position))
        {
            transform.position = Vector3.Lerp(transform.position, _tempPlayerLocation, _currentTime / _attackDashTimeAndDamageTimer);
            _currentTime += Time.deltaTime;
            yield return null;
        }
    }


    //�Ϲ� ����
    IEnumerator PlayerAttack()
    {
        _isPlayerAttack = true;
        //���� ������ �� �÷��̾ ������ ���ݹ����� ���� ��� ���� ����
        //for (int i = 0; i < _attackFrame; i++)
        //{
        //    yield return null;
        //}

        StartCoroutine(AttackGettingCloseToPlayer());
        yield return new WaitForSeconds(1f);

        if (CheckPlayer())
        {
            Debug.Log("�÷��̾ ���ݹ޾Ҵ�");
            player.GetComponent<Player>().TakeDamage(_attack);
        }

        //���� ����
        if (player.activeSelf)
        {
            StartCoroutine(PlaySoundEffect(3));
        }

        //_isPlayerAttack = false;

        _onAttack = false;
    }


    protected bool _isAttackDelay = false;
    //���� ��Ÿ�� ����
    IEnumerator AttackDelay()
    {
        _isAttackDelay = true;
        _enableAttack = false;
        yield return new WaitForSeconds(_attackDelay);
        _enableAttack = true;
        _onAttack = false;
        _isAttackDelay = false;

        //isPlayerAttack false ��ȯ�� ��� �����ؾߵ�
        _isPlayerAttack = false;
        _enemyCollider.size = new Vector3(_colliderSize, _colliderSize, _colliderSize);
    }

    protected bool _isSpellDelay = false;
    //���� ��Ÿ�� ����
    IEnumerator SpellDelay()
    {
        _isSpellDelay = true;
        _enableAttack = false;
        _enableSpell = false;
        yield return new WaitForSeconds(_attackDelay);

        _enableAttack = true;
        _onAttack = false;
        yield return new WaitForSeconds(_spellDelay - _attackDelay);
        _enableSpell = true;
        _isSpellDelay = false;
    }

    //private void Update()
    //{
    //    //CheckPlayer();
    //}


    //�������� ����
    public void OnTakeDamage(float damage)
    {
        if (_isPlayerAttack)
        {
            StopCoroutine("PlayerAttack");
            _enableAttack = true;
        }
        if (_isPlayerSpellAttack)
        {
            StopCoroutine("PlayerSpellAttack");
            //Debug.Log("�÷� �����");
            if (_boss == false)
            {
                _enableSpell = true;
            }
            _enableAttack = true;
        }
        if (_isSpellDelay && _boss == false)
        {
            StopCoroutine("SpellDelay");
            _enableAttack = true;
        }
        if (_isAttackDelay)
        {
            StopCoroutine("AttackDelay");
            if (_boss == false)
            {
                _enableSpell = true;
            }
            _enableAttack = true;
        }

        _onAttack = false;


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
            StartCoroutine(Hurt());
        }

        //isPlayerAttack false ��ȯ�� ��� �����ؾߵ�
        _isPlayerAttack = false;
        _enemyCollider.size = new Vector3(_colliderSize, _colliderSize, _colliderSize);
    }

    protected IEnumerator Hurt()
    {
        _animator.SetBool("_hurt", true);
        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
        _animator.SetBool("_hurt", false);
    }

    //�ǰݽ� ���� �Ұ�
    protected IEnumerator StunDelay()
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
        gameObject.GetComponent<BoxCollider>().enabled = false;



        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
        //gameObject.SetActive(false);

        //��ȯ���� ���� ������ ��� ��ȯ�� �ƴϹǷ� ���� ����
        if (_summon == false)
        {
            // Ǯ ��ȯ
            if (_isElite)
            {
                EnemyPool.Instance.ReturnEliteEnemy(gameObject);
            }
            else
            {
                EnemyPool.Instance.ReturnEnemy(gameObject);
            }

            //�� ����� ���� �� ��ü�� ����
            GameStateManager.Instance.CurrentEnemyCount--;
            //�� ����� �̺�Ʈ ȣ��
            GameStateManager.Instance.OnEnemyDied.Invoke();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


    protected IEnumerator PlaySoundEffect(int _audioNum)
    {
        if (_audioClips[_audioNum] != null)
        {
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