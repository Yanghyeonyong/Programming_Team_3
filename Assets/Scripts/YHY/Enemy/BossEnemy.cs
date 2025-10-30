using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossEnemy : Enemy
{

    [Header("Summon Settings")]
    [SerializeField] private GameObject _monsterPrefab; // ��ȯ�� ���� ������
    [SerializeField] private int _monsterCount = 5;     // �� ���� ��ȯ�� ��
    [SerializeField] private float _summonRadius = 1f;   // ���� �ֺ� ��ȯ �ݰ�
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
    //    //Debug.Log("�ߵ�");
    //    ////����� �ڵ� ������� ����
    //    //for (int i = 0; i < _monsterCount; i++)
    //    //{
    //    //    // ���� �ֺ� ���� ��ġ 
    //    //    Vector3 spawnPos = transform.position +
    //    //                       new Vector3(Random.Range(-_summonRadius, _summonRadius), 0, Random.Range(-_summonRadius, _summonRadius));
    //    //    //��Ȱ��ȭ�� ��ü �ִ��� Ȯ��
    //    //    GameObject _summonObject = CheckIsActive(_summons);

    //    //    //������ ���� ��ȯ
    //    //    if (_summonObject == null)
    //    //    {

    //    //        GameObject _summon = Instantiate(_monsterPrefab, spawnPos, Quaternion.identity);
    //    //        _summons.Add(_summon);
    //    //    }
    //    //    //������ �����ͼ� �ٽ� ���
    //    //    else
    //    //    {
    //    //        _summonObject.transform.position = spawnPos;
    //    //        _summonObject.SetActive(true); 
    //    //    }
    //    //    yield return new WaitForSeconds(0.3f); // �������� �ణ�� ������
    //    //}

    //    //_onAttack = false;
    //    //_isPlayerSpellAttack = false;
    //    Debug.Log("�ߵ�1");
    //    StartCoroutine("FireBall");
    //    yield return null;
    //}
    IEnumerator PlayerSpellAttack()
    {

        _isPlayerSpellAttack = true;
        if (_enableSecondSpell && CheckPlayerByBoss())
        {
            //Vector3 _originSpellSize = _spellObject.transform.localScale;
            //�� ��° ���� ��Ÿ�� ����
            StartCoroutine("SecondSpellDelay");

            //��¡�� ���� �Ұ�
            _isAttackDelay = true;
            _enableAttack = false;

            //���̾��� Ȱ��ȭ
            _spellObject.SetActive(true);

            //���̾��� ũ�� ����
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
                    Debug.Log("�¾Ƽ� �����");
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
            Debug.Log("�ߵ�");
            //����� �ڵ� ������� ����
            for (int i = 0; i < _monsterCount; i++)
            {
                // ���� �ֺ� ���� ��ġ 
                Vector3 spawnPos = transform.position +
                                   new Vector3(Random.Range(-_summonRadius, _summonRadius), 0, Random.Range(-_summonRadius, _summonRadius));
                //��Ȱ��ȭ�� ��ü �ִ��� Ȯ��
                GameObject _summonObject = CheckIsActive(_summons);

                //������ ���� ��ȯ
                if (_summonObject == null)
                {

                    GameObject _summon = Instantiate(_monsterPrefab, spawnPos, Quaternion.identity);
                    _summons.Add(_summon);
                }
                //������ �����ͼ� �ٽ� ���
                else
                {
                    _summonObject.transform.position = spawnPos;
                    _summonObject.SetActive(true);
                }
                yield return new WaitForSeconds(0.3f); // �������� �ణ�� ������
            }
        }
        _onAttack = false;
        _isPlayerSpellAttack = false;
        //Debug.Log("�ߵ�1");
        //StartCoroutine("FireBall");
        yield return null;
    }

    //��Ȱ��ȭ�� ��ü ��ȯ
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
    //�÷��̾ ���� ������ �ִ��� üũ
    private bool CheckPlayerByBoss()
    {

        Ray ray = new Ray(transform.position + Vector3.up * 0.2f, transform.right * _spellRange);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _spellRange, layerMask))
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
