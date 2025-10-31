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
    [SerializeField] protected float _attackDelay;
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
    [SerializeField] private float _isBlockedCheckDistance = 1f;
    [SerializeField] private float _attackRayTiming = 1f;
    //private bool _isAttacking = false;
    BoxCollider _enemyCollider;
    float _colliderSize;
    float _moveDistancePreserve;
    [SerializeField] private float _attackDashCloseDistance = 0.65f;



    //�÷��̾�
    GameObject player;

    //�ִϸ��̼��� ���� �ִϸ�����
    Animator _animator;

    //���� ������ ��� üũ
    private bool _hitByWall = false;
    //���� ����(z�� ����) �Ѿ �� üũ
    private bool _hitByWallUp = false;

    //����ĳ��Ʈ �浹 ����
    protected int layerMask;

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
        
        //�� ������ �ƹ��͵� ���� �ذ� ����
        StopAllCoroutines();

        _onStun = false;
        _onAttack = false;
        _enableAttack = true;
        _enableSpell = true;
        _isPlayerAttack = false;
        _isPlayerSpellAttack = false;


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
        //�밢�� ����
        while (true && _curHp >0 )
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
                if (_isPlayerAttack == false && _onStun ==false)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed);

                }
                else if (_isPlayerAttack == true)
                {

                    //if (_enemyCollider != null)
                    //{
                    //    _enemyCollider.size = new Vector3(_colliderSize * 0.5f, _colliderSize, _colliderSize);
                    //    //_moveDistance = _moveDistance * 0.5f;
                    //    //transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed);
                    //}

                    

                    //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, _moveSpeed);
                }

                    //Debug.Log("����");
                

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
        //�밢�� ���� ���� �õ�
        if (CheckPlayer() && _boss==false) //&& (transform.position.z- player.transform.position.z)>= -0.05f&& (transform.position.z-player.transform.position.z) <=0.05f )
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


                    //StartCoroutine(PlayerAttack());
                    //StartCoroutine(AttackDelay());
                    StartCoroutine("PlayerAttack");
                    StartCoroutine("AttackDelay");
                }
                //������ ���� ���� ���
                else
                {
                    if (_enableSpell == true)
                    {
                        _animator.SetTrigger("_cast");
                        //StartCoroutine(PlayerSpellAttack());
                        //StartCoroutine(SpellDelay());
                        StartCoroutine("PlayerSpellAttack");
                        StartCoroutine("SpellDelay");
                    }
                    else
                    {
                        _animator.SetTrigger("_attack");
                        //StartCoroutine(PlayerAttack());
                        //StartCoroutine(AttackDelay());
                        StartCoroutine("PlayerAttack");
                        StartCoroutine("AttackDelay");
                    }
                }
            }
        }
        else if (_boss == true)
        {

            if (_enableAttack == true && _onStun == false)
            {
                _onAttack = true;

                if (_enableSpell == true)
                {
                    _animator.SetTrigger("_cast");
                    StartCoroutine("PlayerSpellAttack");
                    StartCoroutine("SpellDelay");
                }
                else
                {
                    if (CheckPlayer())
                    {
                        _animator.SetTrigger("_attack");
                        StartCoroutine("PlayerAttack");
                        StartCoroutine("AttackDelay");
                    }
                    //���� �� ���� ���Ͱ� �� �ڸ����� ���ߴ� ���� ����
                    else
                    {
                        _onAttack = false;
                    }
                }
            }
        }
    }



    //�÷��̾ ���� ������ �ִ��� üũ
    private bool CheckPlayer()
    {
        //������ ��������
        //if (player.transform.position.z != transform.position.z)
        //{
        //    return false;
        //}
        //if (_curHp <= 0)
        //{
        //    return false;
        //}
        //Vector3 _tempVector = transform.right;

        //float _checkLeft = Vector3.Dot(transform.right.normalized, Vector3.left);
        //float _checkRight = Vector3.Dot(transform.right.normalized, Vector3.right);
        ////�밢�� ���ݹ���
        //if ( !(_checkLeft<-0.99f  && _checkRight <-0.99f))
        //{
        //    return false;
        //}


        Ray ray = new Ray(transform.position + Vector3.up * 0.2f, transform.right * _attackRange);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _attackRange, layerMask))
        {
            //Debug.Log(hit.collider.gameObject.name);
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


    public bool _isPlayerSpellAttack = false;
    //[SerializeField] private int _attackFrame = 1;
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
    private bool _isPlayerAttack = false;
    
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

        //_rigidbody.AddForce(transform.right * 2f, ForceMode.Impulse);
        //float _impactTime = 0f;
        //float _currentTime = 0f;



        StartCoroutine(AttackGettingCloseToPlayer());
        yield return new WaitForSeconds(_attackRayTiming);

        if (CheckPlayer())
        {
            Debug.Log("�÷��̾ ���ݹ޾Ҵ�");
            //if (player != null)
            //{
                player.GetComponent<Player>().TakeDamage(_attack);
            //}
        }
        //yield return new WaitForSeconds(_attackRayTiming);

        //���� ����
        if (player.activeSelf)
        {
            StartCoroutine(PlaySoundEffect(3));
        }
            //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, _moveSpeed);

        //if (CheckPlayer())
        //{
        //    Debug.Log("�÷��̾ ���ݹ޾Ҵ�");
        //    player.GetComponent<Player>().TakeDamage(_attack);
        //}

        //make sure _isPlayerAttack switches to false
        //yield return new WaitForSeconds(_attackDelay);
        //_isPlayerAttack = false;
        _onAttack = true;

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


    protected bool _isTakeDamage=false;

    //�������� ����
    public void OnTakeDamage(float damage)
    {
        _isTakeDamage = true;

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

        if (_boss)
        {
            _spellObject.SetActive(false);
        }

        _onAttack = false;


        StartCoroutine(PlaySoundEffect(0));
        _curHp -= damage;
        if (_curHp <= 0)
        {
            _curHp = 0;
            StartCoroutine(Die());

        }
        StartCoroutine(StunDelay());
        if(_curHp >0)
        {
            StartCoroutine(Hurt());
        }

        //isPlayerAttack false ��ȯ�� ��� �����ؾߵ�
        _isPlayerAttack = false;
        _enemyCollider.size = new Vector3(_colliderSize, _colliderSize, _colliderSize);

        _isTakeDamage = false;
    }

    IEnumerator Hurt()
    {
        _animator.SetBool("_hurt", true);
        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
        _animator.SetBool("_hurt", false);
        
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
        StopCoroutine(StunDelay());
        StopCoroutine(Hurt());
        StopCoroutine(AttackDelay());
        StopCoroutine(AttackGettingCloseToPlayer());
        StopCoroutine(PlayerAttack());
        StopCoroutine(SpellDelay());
        StopCoroutine(PlayerSpellAttack());
        StopCoroutine(FindPlayer());
        StopCoroutine(ChasePlayer());
        StartCoroutine(PlaySoundEffect(1));
        //StopCoroutine(ChasePlayer());
        _animator.Play("Death");
        gameObject.GetComponent<BoxCollider>().enabled = false;

        //GameStateManager.Instance.CurrentEnemyCount--;
        //GameStateManager.Instance.OnEnemyDied.Invoke();

        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);

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
            GameStateManager.Instance.CheckStageClear();
        }
        else
        {
            gameObject.SetActive(false);
        }

        //gameObject.SetActive(false);


        //

    }


    IEnumerator PlaySoundEffect(int _audioNum)
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