using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossEnemy : Enemy
{

    [Header("Summon Settings")]
    [SerializeField] private GameObject _monsterPrefab; // 소환할 몬스터 프리팹
    [SerializeField] private int _monsterCount = 5;     // 한 번에 소환할 수
    [SerializeField] private float _summonRadius = 1f;   // 보스 주변 소환 반경
    List<GameObject> _summons=new List<GameObject>();

    [SerializeField] private float _secondSpellDelay = 3f;
    [SerializeField] private bool _enableSecondSpell = true;

    [SerializeField] private float _firaBallCharge = 2f;
    [SerializeField] private float _chargingSpeed = 0.1f;
    //IEnumerator PlayerSpellAttack()
    //{

    //    //_isPlayerSpellAttack = true;

    //    //for (int i = 0; i < _spellFrame; i++)
    //    //{
    //    //    yield return null;
    //    //}
    //    //Debug.Log("발동");
    //    ////진희님 코드 기반으로 구현
    //    //for (int i = 0; i < _monsterCount; i++)
    //    //{
    //    //    // 보스 주변 랜덤 위치 
    //    //    Vector3 spawnPos = transform.position +
    //    //                       new Vector3(Random.Range(-_summonRadius, _summonRadius), 0, Random.Range(-_summonRadius, _summonRadius));
    //    //    //비활성화된 객체 있는지 확인
    //    //    GameObject _summonObject = CheckIsActive(_summons);

    //    //    //없으면 새로 소환
    //    //    if (_summonObject == null)
    //    //    {

    //    //        GameObject _summon = Instantiate(_monsterPrefab, spawnPos, Quaternion.identity);
    //    //        _summons.Add(_summon);
    //    //    }
    //    //    //있으면 가져와서 다시 사용
    //    //    else
    //    //    {
    //    //        _summonObject.transform.position = spawnPos;
    //    //        _summonObject.SetActive(true); 
    //    //    }
    //    //    yield return new WaitForSeconds(0.3f); // 마리마다 약간의 딜레이
    //    //}

    //    //_onAttack = false;
    //    //_isPlayerSpellAttack = false;
    //    Debug.Log("발동1");
    //    StartCoroutine("FireBall");
    //    yield return null;
    //}
    IEnumerator PlayerSpellAttack()
    {

        _isPlayerSpellAttack = true;
        if (_enableSecondSpell && CheckPlayerByBoss())
        {
            //Vector3 _originSpellSize = _spellObject.transform.localScale;
            //두 번째 스펠 쿨타임 적용
            StartCoroutine("SecondSpellDelay");

            //차징중 공격 불가
            _isAttackDelay = true;
            _enableAttack = false;

            //파이어폴 활성화
            _spellObject.SetActive(true);

            //파이어폴 크기 증가
            while (true)
            {
                _spellObject.transform.localScale = new Vector3(_spellObject.transform.localScale.x + _chargingSpeed
                                                                    , _spellObject.transform.localScale.y + _chargingSpeed
                                                                    , _spellObject.transform.localScale.z + _chargingSpeed);
                yield return new WaitForSeconds(0.02f);
                if (_spellObject.transform.localScale.x >= _firaBallCharge)
                {
                    break;
                }
                if (_isTakeDamage)
                {
                    Debug.Log("맞아서 멈춘다");
                    _spellObject.SetActive(false);
                    _spellObject.GetComponent<BossSecondSpell>().CancelSpell();
                    yield break;
                }
            }

            _spellObject.transform.SetParent(null);
            _spellObject.GetComponent<BossSecondSpell>()._onShot = true;
            StartCoroutine(_spellObject.GetComponent<BossSecondSpell>().Shooting());
            

        }
        else
        {
            for (int i = 0; i < _spellFrame; i++)
            {
                yield return null;
            }
            Debug.Log("발동");
            //진희님 코드 기반으로 구현
            for (int i = 0; i < _monsterCount; i++)
            {
                // 보스 주변 랜덤 위치 
                Vector3 spawnPos = transform.position +
                                   new Vector3(Random.Range(-_summonRadius, _summonRadius), 0, Random.Range(-_summonRadius, _summonRadius));
                //비활성화된 객체 있는지 확인
                GameObject _summonObject = CheckIsActive(_summons);

                //없으면 새로 소환
                if (_summonObject == null)
                {

                    GameObject _summon = Instantiate(_monsterPrefab, spawnPos, Quaternion.identity);
                    _summons.Add(_summon);
                }
                //있으면 가져와서 다시 사용
                else
                {
                    _summonObject.transform.position = spawnPos;
                    _summonObject.SetActive(true);
                }
                yield return new WaitForSeconds(0.3f); // 마리마다 약간의 딜레이
            }
        }
        _onAttack = false;
        _isPlayerSpellAttack = false;
        //Debug.Log("발동1");
        //StartCoroutine("FireBall");
        yield return null;
    }

    //비활성화된 객체 반환
    private GameObject CheckIsActive(List<GameObject> _obj)
    {
        foreach (GameObject obj in _obj)
        {
            if (obj.activeSelf == false)
            {
                return obj;
            }
        }
        return null;
    }
    private void OnDisable()
    {
        foreach (GameObject summon in _summons)
        {
            if (summon != null)
            {
                summon.SetActive(false);
            }
        }
    }


    [SerializeField] private bool _finishedChrage=false;

    IEnumerator FireBall()
    {
        while (true) {
            _spellObject.transform.localScale = new Vector3(_spellObject.transform.localScale.x +_chargingSpeed
                                                                , _spellObject.transform.localScale.y + _chargingSpeed
                                                                , _spellObject.transform.localScale.z + _chargingSpeed);
            yield return new WaitForSeconds(0.02f);
            if (_spellObject.transform.localScale.x >= _firaBallCharge)
                break;

        }
        _finishedChrage = true;

        yield return null;
    }

    [SerializeField] private float _spellRange = 1f;
    //플레이어가 레이 범위에 있는지 체크
    private bool CheckPlayerByBoss()
    {

        Ray ray = new Ray(transform.position + Vector3.up * 0.2f, transform.right * _spellRange);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _spellRange, layerMask))
        {
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

    IEnumerator SecondSpellDelay()
    {
        _enableAttack = false;
        _enableSecondSpell = false;
        yield return new WaitForSeconds(_attackDelay);

        _enableAttack = true;
        _onAttack = false;
        yield return new WaitForSeconds(_secondSpellDelay - _attackDelay);
        _enableSecondSpell = true;
    }

}
