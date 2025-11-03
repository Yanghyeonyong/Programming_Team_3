using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    //몬스터 Hp
    [SerializeField] private float _maxHp;
    [SerializeField] protected float _curHp;

    //몬스터 공격력
    [SerializeField] private float _attack;
    [SerializeField] private float _spellAttack;
    protected bool _onAttack = false;

    //몬스터 공격 범위
    [SerializeField] private float _attackRange;
    [SerializeField] protected GameObject _spellObject;

    //몬스터 공격 활성화
    protected bool _enableAttack = true;
    protected bool _enableSpell = true;
    [SerializeField] protected float _attackDelay;
    [SerializeField] private float _spellDelay;

    //몬스터 피격 후 경직
    [SerializeField] private float _stunDelay;
    private bool _onStun = false;

    //몬스터 이동 속도
    [SerializeField] private float _moveSpeed;
    //몬스터와 플레이어 거리
    [SerializeField] private float _moveDistance;

    //최정욱 충돌 튕겨져나가는 것 방지, 공격 타이밍, 공겨시 움직임 멈춤
    private Rigidbody _rigidbody;
    [SerializeField] private float _isBlockedCheckDistance = 1f;
    [SerializeField] private float _attackRayTiming = 1f;
    //private bool _isAttacking = false;
    BoxCollider _enemyCollider;
    float _colliderSize;
    float _moveDistancePreserve;
    [SerializeField] private float _attackDashCloseDistance = 0.65f;


    [SerializeField] private bool _boss = false;
    protected bool _isTakeDamage = false;


    //플레이어
    GameObject player;

    //애니메이션을 위한 애니메이터
    Animator _animator;

    //벽에 막혔을 경우 체크
    private bool _hitByWall = false;
    //벽을 위로(z축 위로) 넘어갈 지 체크
    private bool _hitByWallUp = false;

    //레이캐스트 충돌 방지

    public int layerMask;


    private AudioSource _audioSource;
    //0 : 피격
    //1 : 사망
    //2 : 걷기
    //3 : 공격
    //4 : 특수 공격(스펠)
    [SerializeField] List<AudioClip> _audioClips;


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


    //private void Awake()
    //{
    //    Init();
    //}
    private void OnEnable()
    {
        StartCoroutine(FindPlayer());
        //Init();

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
        //Debug.Log($"플레이어 찾음 {player.name}");
        Init();
        //Debug.Log("적 초기화 완료");
    }

    private void Init()
    {
        
        //재 스폰시 아무것도 안함 해결 정욱
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
        //레이가 플레이어와 벽만 충돌 체크하도록 설정
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

    //플레이어 추적
    IEnumerator ChasePlayer()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
        //대각선 방지

        while (true && _curHp >0)

        {

            Vector3 target = new Vector3(0, 0, 0);

            //벽에 부딪히지 않았을 경우

            if (_hitByWall == false && !CheckPlayer() && _onAttack == false)
            {
                //
                StartCoroutine(PlaySoundEffect(2));
                //
                _animator.SetBool("_walk", true);

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

                //float _distance = Vector3.Distance(transform.position, target);
                Vector3 _direction = (target - transform.position).normalized;

                if (Physics.Raycast(transform.position + Vector3.up * 0.2f, _direction, out RaycastHit hit, _isBlockedCheckDistance, layerMask + (1 << LayerMask.NameToLayer("Enemy"))))

                {
                    if (!hit.collider.gameObject.CompareTag("Wall"))
                    //Debug.Log("벽 앞에서 멈춤");
                    {
                        target = transform.position;
                    }
                }
                //y축 이동 방지
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

                    //Debug.Log("멈춤");
                

            }
            else
            {
                _animator.SetBool("_walk", false);
            }

            //Debug.Log("right before attack");
            Attack();

            if (_hitByWall == true && _hitByWallUp == true)
            {
                //벽에 맞히면 위로 넘어가도록 구현
                target = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
                transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed);
            }
            else if (_hitByWall == true && _hitByWallUp == false)
            {
                //벽에 맞히면 위로 넘어가도록 구현
                target = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
                transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed);
            }

            yield return null;
        }
    }

    //벽 충돌시 이동 방향 체크
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
    //벽 충돌에서 벗어날 경우
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            _hitByWall = false;
        }


        //_rigidbody = GetComponent<Rigidbody>();

        

        //StartCoroutine(ChasePlayer());

    }



    //공격 동작 시행
    private void Attack()
    {
        //Debug.Log("attack function called");
        //대각선 공격 방지 시도

        if (CheckPlayer() && _boss ==false) //&& (transform.position.z- player.transform.position.z)>= -0.05f&& (transform.position.z-player.transform.position.z) <=0.05f )

        {
            //Debug.Log("player in attack range");
            if (_enableAttack == true && _onStun == false)
            {
                _onAttack = true;
                //추가적인 스펠이 없는 적의 경우
                if (_spellObject == null)
                {
                    //_isAttacking = true;
                    _animator.SetTrigger("_attack");


                    //StartCoroutine(PlayerAttack());
                    //StartCoroutine(AttackDelay());
                    StartCoroutine("PlayerAttack");
                    StartCoroutine("AttackDelay");
                }
                //스펠을 가진 적의 경우
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



        else if (_boss ==true)
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
                    //시작 시 보스 몬스터가 제 자리에서 멈추는 현상 수정
                    else
                    {
                        _onAttack = false;
                    }
                }
            }
        }

    }



    //플레이어가 레이 범위에 있는지 체크
    private bool CheckPlayer()
    {
        //최정욱 수정사합
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
        ////대각선 공격방지
        //if ( !(_checkLeft<-0.99f  && _checkRight <-0.99f))
        //{
        //    return false;
        //}


        Ray ray = new Ray(transform.position + Vector3.up * 0.2f, transform.right * _attackRange);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _attackRange, layerMask))
        {
            //Debug.Log(hit.collider.gameObject.name);
            //플레이어 레이어 숫자는 3
            if (hit.collider.gameObject.layer == 3)
            {
                return true;
            }
        }
        else
        {
            //Debug.Log("아무것도 안맞음");
        }
        return false;
    }

    //플레이어와 근접시 속도 증가
    public void PlusMoveSpeed(float _plusMoveSpeed)
    {
        _moveSpeed += _plusMoveSpeed;
    }


    public bool _isPlayerSpellAttack = false;

    [SerializeField] private int _attackFrame = 1;
    [SerializeField] public int _spellFrame = 1;

    //스펠 공격
    public virtual IEnumerator PlayerSpellAttack()

    {
        _isPlayerSpellAttack = true;



        //스펠 사운드
        StartCoroutine(PlaySoundEffect(4));
        Vector3 _tempPlayerLocation = player.transform.position;

        for (int i = 0; i < _spellFrame; i++)
        {
            yield return null;
        }

        _spellObject.GetComponent<Spell>()._tempPlayerPosition = _tempPlayerLocation + new Vector3 (0,0.7f,0);
            _spellObject.SetActive(true);
        //yield return new WaitForSeconds(1f);
        RaycastHit _spellhit;


        while (_spellObject.GetComponent<Spell>()._isAtEndOfAnimation == false)
        {
            
            yield return null;
        }


        if (Physics.Raycast(_tempPlayerLocation + new Vector3(0, 2f, 0), Vector3.down, out _spellhit, 3f, layerMask))
        {
            //Debug.Log("Spell Raycast Hit : " + _spellhit.collider.gameObject.name);
            if (_spellhit.collider.gameObject.tag == "Player")
            {
                //Debug.Log("플레이어가 스펠 공격받았다");
                //if (player != null)
                //{
                player.GetComponent<Player>().TakeDamage(_spellAttack);
                //}
            }
            //_spellObject.transform.position = new Vector3(player.transform.position.x, _spellObject.transform.position.y, player.transform.position.z);
            //player.GetComponent<Player>().TakeDamage(_spellAttack);

        }
        _onAttack = false;
        _isPlayerSpellAttack = false;
    }

    [SerializeField] private float _attackDashTimeAndDamageTimer = 0.5f;
    private bool _isPlayerAttack = false;
    
    //공격하면서 플레이어 가까워지고 있게

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
    
    

    //일반 공격
    IEnumerator PlayerAttack()
    {
        _isPlayerAttack = true;

        _onAttack = true;

        //일정 프레임 후 플레이어가 여전히 공격범위에 있을 경우 피해 입음
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
            Debug.Log("플레이어가 공격받았다");
            //if (player != null)
            //{
                player.GetComponent<Player>().TakeDamage(_attack);
            //}
        }
        //yield return new WaitForSeconds(_attackRayTiming);


        //공격 사운드
        if (player.activeSelf)
        {
            StartCoroutine(PlaySoundEffect(3));
        }
        //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, _moveSpeed);


        //if (CheckPlayer())
        //{
        //    Debug.Log("플레이어가 공격받았다");
        //    player.GetComponent<Player>().TakeDamage(_attack);
        //}

        //make sure _isPlayerAttack switches to false
        //yield return new WaitForSeconds(_attackDelay);
        //_isPlayerAttack = false;

        //_onAttack = false;
    }
    

    public bool _isAttackDelay = false;

    //공격 쿨타임 적용
    IEnumerator AttackDelay()
    {
        _isAttackDelay = true;
        _enableAttack = false;
        yield return new WaitForSeconds(_attackDelay);
        _enableAttack = true;
        _onAttack = false;
        _isAttackDelay = false;
        
        //isPlayerAttack false 변환시 얘는 동반해야됨
        _isPlayerAttack = false;
        _enemyCollider.size = new Vector3(_colliderSize, _colliderSize, _colliderSize);
    }


    private bool _isSpellDelay = false;

    //스펠 쿨타임 적용
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


    protected bool _isTakeDamage=false;

    //데미지를 입음
    public void OnTakeDamage(float damage)
    {
        _isTakeDamage = true;


    private void Update()
    {
        //CheckPlayer();
    }


    //데미지를 입음
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

            Debug.Log("플레 멈췄다");
            _enableSpell = true;

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


        if(_boss)

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

        //isPlayerAttack false 변환시 얘는 동반해야됨
        _isPlayerAttack = false;
        //_enemyCollider.size = new Vector3(_colliderSize, _colliderSize, _colliderSize);

        _isTakeDamage = false;
    }

    IEnumerator Hurt()
    {
        _animator.SetBool("_hurt", true);
        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
        _animator.SetBool("_hurt", false);
        
    }

    //피격시 공격 불가
    IEnumerator StunDelay()
    {
        _onStun = true;
        yield return new WaitForSeconds(_stunDelay);
        _onStun = false;
    }


    [SerializeField] private bool _isElite = false;
    [SerializeField] private  bool _summon = false;

    //몬스터 사망
    public virtual IEnumerator Die()

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

        if (GameStateManager.Instance != null && _boss ==true)
        {
            GameStateManager.Instance._isBossAlive = false;
        }



        if (_summon == false)
        {
            // 풀 반환
            if (_isElite)
            {
                EnemyPool.Instance.ReturnEliteEnemy(gameObject);
            }
            else
            {
                EnemyPool.Instance.ReturnEnemy(gameObject);
            }

            //적 사망시 현재 적 개체수 감소
            GameStateManager.Instance.CurrentEnemyCount--;
            //적 사망시 이벤트 호출
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



    public IEnumerator PlaySoundEffect(int _audioNum)
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