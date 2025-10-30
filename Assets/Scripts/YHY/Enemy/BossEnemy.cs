using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{

    [Header("Summon Settings")]
    [SerializeField] private GameObject _skeletonPrefab; // 소환할 몬스터 프리팹
    [SerializeField] private int _skeletonCount = 5;     // 한 번에 소환할 수
    [SerializeField] private float _summonRadius = 1f;   // 보스 주변 소환 반경
    List<GameObject> _summons=new List<GameObject>();
    IEnumerator PlayerSpellAttack()
    {

        _isPlayerSpellAttack = true;

        for (int i = 0; i < _spellFrame; i++)
        {
            yield return null;
        }
        Debug.Log("발동");
        //진희님 코드 기반으로 구현
        for (int i = 0; i < _skeletonCount; i++)
        {
            // 보스 주변 랜덤 위치 
            Vector3 spawnPos = transform.position +
                               new Vector3(Random.Range(-_summonRadius, _summonRadius), 0, Random.Range(-_summonRadius, _summonRadius));
            //비활성화된 객체 있는지 확인
            GameObject _summonObject = CheckIsActive(_summons);

            //없으면 새로 소환
            if (_summonObject == null)
            {
                
                GameObject _summon = Instantiate(_skeletonPrefab, spawnPos, Quaternion.identity);
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

        _onAttack = false;
        _isPlayerSpellAttack = false;
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
            summon.SetActive(false);
        }
    }
}
