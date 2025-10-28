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
    [SerializeField] private float _spellAttack;
    private bool _onAttack = false;

    //몬스터 공격 범위
    [SerializeField] private float _attackRange;
    [SerializeField] private GameObject _spellObject;

    //몬스터 공격 활성화
    private bool _enableAttack = true;
    private bool _enableSpell = true;
    [SerializeField] private float _attackDelay;
    [SerializeField] private float _spellDelay;

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

    //벽에 막혔을 경우 체크
    private bool _hitByWall = false;
    //벽을 위로(z축 위로) 넘어갈 지 체크
    private bool _hitByWallUp = false;

    //레이캐스트 충돌 방지
    private int layerMask;

    private AudioSource _audioSource;
    //0 : 피격
    //1 : 사망
    //2 : 걷기
    //3 : 공격
    //4 : 특수 공격(스펠)
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
        //레이가 플레이어와 벽만 충돌 체크하도록 설정
        layerMask = (1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("Wall"));

        _curHp = _maxHp;
        player = GameObject.Find("Player");
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
        while (true)
        {

            Vector3 target = new Vector3(0, 0, 0);

            //벽에 부딪히지 않았을 경우
            if (_hitByWall == false && _onAttack == false)
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

                //y축 이동 방지
                transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed);
            }
            else
            {
                _animator.SetBool("_walk", false);
            }

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
    }
    //벽 충돌에서 벗어날 경우
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            _hitByWall = false;
        }
    }

    //공격 동작 시행
    private void Attack()
    {
        if (CheckPlayer())
        {
            if (_enableAttack == true && _onStun == false)
            {
                _onAttack = true;
                //추가적인 스펠이 없는 적의 경우
                if (_spellObject == null)
                {
                    //_onAttack = true;
                    _animator.SetTrigger("_attack");
                    StartCoroutine(PlayerAttack());
                    StartCoroutine(AttackDelay());
                }
                //스펠을 가진 적의 경우
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

    //플레이어가 레이 범위에 있는지 체크
    private bool CheckPlayer()
    {
        Ray ray = new Ray(transform.position, transform.right * _attackRange);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _attackRange, layerMask))
        {
            //플레이어 레이어 숫자는 3
            if (hit.collider.gameObject.layer == 3)
            {
                return true;
            }
        }
        return false;
    }

    //플레이어와 근접시 속도 증가
    public void PlusMoveSpeed(float _plusMoveSpeed)
    {
        _moveSpeed += _plusMoveSpeed;
    }

    [SerializeField] private int _attackFrame = 1;
    [SerializeField] private int _spellFrame = 1;

    //스펠 공격
    IEnumerator PlayerSpellAttack()
    {
        //일정 프레임 후 플레이어가 여전히 공격범위에 있을 경우 피해 입음
        for (int i = 0; i < _spellFrame; i++)
        {
            yield return null;
        }

        //스펠 사운드
        StartCoroutine(PlaySoundEffect(4));

        if (CheckPlayer())
        {
            _spellObject.transform.position = new Vector3(player.transform.position.x, _spellObject.transform.position.y, player.transform.position.z);
            _spellObject.SetActive(true);
            player.GetComponent<Player>().TakeDamage(_spellAttack);
        }
    }
    //일반 공격
    IEnumerator PlayerAttack()
    {

        //일정 프레임 후 플레이어가 여전히 공격범위에 있을 경우 피해 입음
        for (int i = 0; i < _attackFrame; i++)
        {
            yield return null;
        }

        //공격 사운드
        StartCoroutine(PlaySoundEffect(3));
        if (CheckPlayer())
        {
            player.GetComponent<Player>().TakeDamage(_attack);
        }
    }

    //공격 쿨타임 적용
    IEnumerator AttackDelay()
    {
        _enableAttack = false;
        yield return new WaitForSeconds(_attackDelay);
        _enableAttack = true;
        _onAttack = false;
    }

    //스펠 쿨타임 적용
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

    //데미지를 입음
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

            Debug.Log("사운드 " + _audioNum);
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