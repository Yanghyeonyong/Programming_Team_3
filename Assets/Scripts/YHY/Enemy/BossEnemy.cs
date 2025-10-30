using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{

    [Header("Summon Settings")]
    [SerializeField] private GameObject _skeletonPrefab; // ��ȯ�� ���� ������
    [SerializeField] private int _skeletonCount = 5;     // �� ���� ��ȯ�� ��
    [SerializeField] private float _summonRadius = 1f;   // ���� �ֺ� ��ȯ �ݰ�
    List<GameObject> _summons=new List<GameObject>();
    IEnumerator PlayerSpellAttack()
    {

        _isPlayerSpellAttack = true;

        for (int i = 0; i < _spellFrame; i++)
        {
            yield return null;
        }
        Debug.Log("�ߵ�");
        //����� �ڵ� ������� ����
        for (int i = 0; i < _skeletonCount; i++)
        {
            // ���� �ֺ� ���� ��ġ 
            Vector3 spawnPos = transform.position +
                               new Vector3(Random.Range(-_summonRadius, _summonRadius), 0, Random.Range(-_summonRadius, _summonRadius));
            //��Ȱ��ȭ�� ��ü �ִ��� Ȯ��
            GameObject _summonObject = CheckIsActive(_summons);

            //������ ���� ��ȯ
            if (_summonObject == null)
            {
                
                GameObject _summon = Instantiate(_skeletonPrefab, spawnPos, Quaternion.identity);
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

        _onAttack = false;
        _isPlayerSpellAttack = false;
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
            summon.SetActive(false);
        }
    }
}
